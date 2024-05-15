using AutoMapper;
using Microsoft.AspNetCore.Http;
using MS.Core.Authorization;
using MS.Core.DTOs;
using MS.Core.Entities;
using MS.Core.Entities.Auth;
using MS.Core.Exceptions;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Services;
using MS.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MS.Core.MSEnums;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Extensions.Configuration;

namespace MS.Core.Services
{
    public class EmployeeService : BaseService<Employee>, IEmployeeService
    {
        IJwtUtils _tokenUtils;
        IFileTransfer _fileTransfer;
        ICommonFunction _commonFunc;
        IDrawTool _drawTool;
        IConfiguration _config;

        public EmployeeService(IEmployeeRepository repository, IUnitOfWork unitOfWork, IMapper mapper, IJwtUtils tokenUtils, IFileTransfer fileTransfer, ICommonFunction commonFunc, IDrawTool drawTool, IConfiguration config) : base(repository, unitOfWork, mapper)
        {
            _tokenUtils = tokenUtils;
            _fileTransfer = fileTransfer;
            _commonFunc = commonFunc;
            _drawTool = drawTool;
            _config = config;
        }

        public override async Task<int> RemoveAsync(object key)
        {
            UnitOfWork.BeginTransaction();
            var loginUserId = _commonFunc.GetCurrentUserId();
            var loginUserInfo = await UnitOfWork.Users.GetUserInfoResponseById(loginUserId);
            var currentEmployeeInfo = await UnitOfWork.Users.GetUserInfoResponseByEmployeeId(key.ToString());
            if (loginUserInfo.EmployeeId == currentEmployeeInfo.EmployeeId)
            {
                throw new MSException(HttpStatusCode.Forbidden, "Bạn không được phép xóa chính mình.");
            }

            if (loginUserInfo.HighestRole > currentEmployeeInfo.HighestRole)
            {
                throw new MSException(HttpStatusCode.Forbidden, "Bạn không được phép xóa thành viên này.");
            }
            return  await UnitOfWork.Employees.RemoveAsync(key);
        }

        public override async Task<PagingData> GetFilterPaging(string keyFilter, int limit, int offset, HashSet<ColumnSortInfo> columnSorts = null, HashSet<ColumnFilterInfo> columnFilters = null)
        {
            var colmnsSortString = new string[] { "CreatedDate DESC", "EmployeeCode ASC" };
            var columnsFilterString = new string[] { "EmployeeCode", "FullName", "Mobile" };
            var data = await UnitOfWork.Employees.GetFilterPaging(colmnsSortString, columnsFilterString, keyFilter, limit, offset);
            return data;
        }

        public async Task<int> AddAsync(EmployeeInfo employeeRequest, IFormFile file)
        {
            var newEmployeeId = Guid.NewGuid();
            var employee = Mapper.Map<EmployeeInfo, Employee>(employeeRequest);
            var user = Mapper.Map<EmployeeInfo, User>(employeeRequest);
            employee.EntityState = MSEnums.MSEntityState.ADD;
            employeeRequest.EntityState = MSEnums.MSEntityState.ADD;
            // Kiểm tra thông tin nhân viên:
            await Validate<Employee>(employee);

            // Kiểm tra thông tin tài khoản:
            if (employeeRequest.User != null)
            {
                await Validate<UserRequest>(employeeRequest.User);
            }

            // Kiểm tra xác nhận mật khẩu đã đúng hay chưa?
            if (employeeRequest.User.Password != employeeRequest.User.Repassword)
            {
                AddErrors("RePassword", "Mật khẩu xác nhận không khớp.");
            }

            // Kiểm tra userName có hợp lệ không (không chứa các ký tự đặc biệt)
            var userNameIsValid = CheckUserName(employeeRequest.User.UserName);
            if (!userNameIsValid)
            {
                AddErrors("UserName", "Tên tài khoản không được chứa các ký tự đặc biệt.");
            }
            ///Check cấp quyền:
            await CheckRole(employeeRequest, user);

            // Cập nhật thông tin trước khi cất dữ liệu:
            employee.EmployeeId = newEmployeeId;
            user.UserId = newEmployeeId;
            user.EmployeeId = newEmployeeId;
            user.PasswordHash = _tokenUtils.HashPassword(employeeRequest.User.Password);

            if (Errors.Count == 0)
            {
                UnitOfWork.BeginTransaction();

                // Tạo Avatar mới:
                await ProcessAvatar(employee, file);

                // Thêm nhân viên vào db:
                var rowAddEmployee = await UnitOfWork.Employees.AddAsync(employee);

                // Thêm tài khoản vào Db:
                var rowAddUser = await UnitOfWork.Users.AddAsync(user) > 0;


                if (rowAddEmployee > 0 && rowAddUser)
                {
                    // Cấp quyền:
                    var roleAdd = await AddPermission(employeeRequest, user);
                    if (!roleAdd)
                    {
                        await _fileTransfer.DeleteFileAsync(employee.AvatarLink);
                        throw new MSException(HttpStatusCode.InternalServerError, "Không thể cấp quyền cho thành viên. Vui lòng liên hệ Quản trị viên để được trợ giúp.");
                    }
                    UnitOfWork.Commit();
                    return rowAddEmployee;
                }
                else
                {
                    await _fileTransfer.DeleteFileAsync(employee.AvatarLink);
                    throw new MSException(HttpStatusCode.InternalServerError, "Không thể thêm mới thành viên do lỗi hệ thống, vui lòng liên hệ QTV để được trợ giúp!");
                }
            }
            else
            {
                throw new MSException(HttpStatusCode.BadRequest, Errors);
            }
        }

        public async Task<int> UpdateAsync(EmployeeInfo employeeRequest, IFormFile file)
        {
            var employee = Mapper.Map<EmployeeInfo, Employee>(employeeRequest);
            var user = Mapper.Map<EmployeeInfo, User>(employeeRequest);
            employee.EntityState = MSEnums.MSEntityState.UPDATE;
            employeeRequest.EntityState = MSEnums.MSEntityState.UPDATE;

            // Kiểm tra thông tin nhân viên:
            await Validate<Employee>(employee);

            // Kiểm tra userName có hợp lệ không (không chứa các ký tự đặc biệt)
            var userNameIsValid = CheckUserName(employeeRequest.User.UserName);
            if (!userNameIsValid)
            {
                AddErrors("UserName", "Tên tài khoản không được chứa các ký tự đặc biệt.");
            }

            //Check cấp quyền:
            await CheckRole(employeeRequest, user);
            if (Errors.Count == 0)
            {
                UnitOfWork.BeginTransaction();

                // Nếu cập nhật avatar thì thay mới, xóa cũ:
                if (file != null)
                {
                    await _fileTransfer.DeleteFileAsync(employee.AvatarLink);
                    // Tạo Avatar mới:
                    await ProcessAvatar(employee, file);
                }

                // Cập nhật thông tin nhân viên:
                var rowAddEmployee = await UnitOfWork.Employees.UpdateAsync(employee);

                // Cập nhật thông tin userName:
                await UnitOfWork.Users.UpdateUserName(user.UserId, user.UserName);

                if (rowAddEmployee > 0)
                {
                    // Cập nhật quyền:
                    // Không cho phép nhân viên tự thay đổi quyền của chính mình và cấp quyền cao hơn quyền hiện tại:
                    var userId = CommonFunction.UserId;
                    var userLogin = await UnitOfWork.Users.GetByIdAsync(userId);
                    if (userLogin.EmployeeId != employee.EmployeeId)
                    {
                        var roleAdd = await AddPermission(employeeRequest, user);
                        if (!roleAdd)
                        {
                            await _fileTransfer.DeleteFileAsync(employee.AvatarLink);
                            throw new MSException(HttpStatusCode.InternalServerError, "Không thể cấp quyền cho thành viên. Vui lòng liên hệ Quản trị viên để được trợ giúp.");
                        }
                    }
                    UnitOfWork.Commit();
                    return rowAddEmployee;
                }
                else
                {
                    await _fileTransfer.DeleteFileAsync(employee.AvatarLink);
                    throw new MSException(HttpStatusCode.InternalServerError, "Không thể thêm mới thành viên do lỗi hệ thống, vui lòng liên hệ QTV để được trợ giúp!");
                }
            }
            else
            {
                throw new MSException(HttpStatusCode.BadRequest, Errors);
            }
        }

        private bool CheckUserName(string userName)
        {
            var regexTemplate = @"^[a-zA-Z0-9]+$";
            var isMatch = Regex.IsMatch(userName,regexTemplate);
            return isMatch;
        }
        private async Task CheckRole(EmployeeInfo employeeRequest, User user)
        {
            var roleId = employeeRequest.RoleId;
            if (roleId == null)
            {
                return;
            }
            // Kiểm tra quyền hiện tại được cấp, không cho phép cấp quyền lớn hơn quyền hiện tại:
            var currentRoleNewMember = await UnitOfWork.Roles.GetByIdAsync(roleId); // Quyền của thành viên.
            var roleMySelf = await UnitOfWork.Roles.GetRoleByUserId(CommonFunction.UserId);

            if (currentRoleNewMember.RoleValue < roleMySelf.RoleValue)
            {
                AddErrors("Role", "Không thể cấp quyền cho thành viên lớn hơn quyền của bạn.");
            }
        }
        private async Task<bool> AddPermission(EmployeeInfo employeeRequest, User user)
        {
            var roleId = employeeRequest.RoleId;
            // Khi thực hiện thêm mới thành viên Nếu không có quyền thì cấp quyền mặc định là thành viên:
            if (roleId == null && employeeRequest.EntityState == MSEnums.MSEntityState.ADD)
            {
                var roles = await UnitOfWork.Roles.GetAllAsync();
                roleId = roles.FirstOrDefault(r => r.RoleValue == MSEnums.MSRole.Member)?.RoleId;
            }

            // Cấp quyền dựa vào vị trí:
            var roleUser = new UserRole()
            {
                RoleId = (Guid)roleId,
                UserId = user.UserId
            };
            // Xóa tất cả quyền cũ nếu có:
            if (employeeRequest.EntityState == MSEnums.MSEntityState.UPDATE)
            {
                await UnitOfWork.Roles.DeleteAllRoleByUserId(user.UserId);
            }
            var roleAdd = await UnitOfWork.UserRoles.AddAsync(roleUser) > 0;

            return roleAdd;
        }

        private async Task ProcessAvatar(Employee employee, IFormFile file)
        {
            if (file != null && file.Length > 0)
            {

                // Dung lượng File được phép tải (lấy từ app config):
                var maxSizeAvatarConfig = _config["MaxSizeAvatar"];
                int maxSizeConfig = 10000;//Size = 1 MB 
                if (maxSizeAvatarConfig != null)
                {
                    maxSizeConfig = int.Parse(maxSizeAvatarConfig);
                }

                int maxContentLength = 1024 * 1024 * maxSizeConfig;  

                // Định dạng File được phép tải (lấy từ app Config): 
                //int fileType = int.Parse(CommonUtility.GetAppSettingByKey("AvatarImgExtensionsAllowed"));
                IList<string> allowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };

                // Tên File
                var ext = file.FileName.Substring(file.FileName.LastIndexOf('.'));

                // Phần mở rộng của File (Ex: .jpg, .png...)
                var extension = ext.ToLower();

                // Kiểm tra tên file và định dạng File có hợp lệ không:
                if (!allowedFileExtensions.Contains(extension))
                {
                    var message = string.Format("Vui lòng chỉ chọn File có định dạng .jpg,.gif,.png.");
                    throw new MSException(HttpStatusCode.BadRequest, message);
                }
                // Kiểm tra dung lượng File:
                else if (file.Length > maxContentLength)
                {
                    var newFileResize = _drawTool.ResizeImg(file);
                    employee.AvatarLink = await _fileTransfer.UploadFileAsync(newFileResize, "avatars", employee.EmployeeId.ToString());
                    //var message = string.Format("Vui lòng chọn File có dung lượng tối đa 1 MB.");
                    //throw new System.InvalidOperationException("Vui lòng chọn File có dung lượng tối đa 1 MB");
                    //throw new MSException(HttpStatusCode.BadRequest, message);
                }
                else
                {
                    // Upload file sang server files::
                    // Thông tin server file xem trong appsetting.json:
                    employee.AvatarLink = await _fileTransfer.UploadFileAsync(file, "avatars", employee.EmployeeId.ToString());
                }
            }
            if (file == null && String.IsNullOrEmpty(employee.AvatarLink))
            {
                // Không có Avatar thì tự vẽ mới:
                //var imgDraw = new ImgDraw();
                //var newImg = imgDraw.Draw(employee.FullName);
                //using MemoryStream ms = new();
                //newImg.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                //HttpResponseMessage result = new(HttpStatusCode.OK);
                //var newFile = new FormFile(ms, 0, ms.ToArray().Length, "avatars", "avatar.png");
                var textImg = "MS";
                if (!String.IsNullOrEmpty(employee.FullName))
                {
                    var nameSplit = employee.FullName.Split(" ");
                    var length = nameSplit.Length;
                    var first = nameSplit[0].Substring(0, 1);
                    var last = nameSplit[length - 1].Substring(0, 1);
                    textImg = $"{first}{last}".ToUpper();
                }
                var newFile = _drawTool.MakeImgText(textImg);
                employee.AvatarLink = await _fileTransfer.UploadFileAsync(newFile, "avatars", employee.EmployeeId.ToString());
            }
        }

    }
}
