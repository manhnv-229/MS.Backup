using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MS.Core.Authorization;
using MS.Core.DTOs;
using MS.Core.Entities;
using MS.Core.Entities.Auth;
using MS.Core.Exceptions;
using MS.Core.Interface.Service;
using MS.Core.Interfaces;
using MS.Core.MSEnums;
using MS.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static MS.Core.Interfaces.IEmailService;

namespace MS.Core.Services
{
    public class UserService : BaseService<User>, IUserService
    {
        IUserRepository _repository;
        IJwtUtils _jwtUtils;
        readonly IEmailService _emailService;

        public UserService(IUserRepository repository, IJwtUtils jwtUtils, IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService) : base(repository, unitOfWork, mapper)
        {
            _repository = repository;
            _jwtUtils = jwtUtils;
            _emailService = emailService;
        }
        public async Task<int> Register(RegisterModel registerModel)
        {

            var addOrgSuccess = false;
            var addEmployeeSuccess = false;
            var addUserSuccess = false;
            var addRoleSuccess = false;

            // Mã đơn vị mới:
            var newGuidId = Guid.NewGuid();
            registerModel.OrganizationId = newGuidId;
            registerModel.Organization.OrganizationId = newGuidId;

            var newEmployee = Mapper.Map<RegisterModel, Employee>(registerModel);
            newEmployee.EmployeeId = newGuidId;
            newEmployee.EmployeeCode = "NV-001";
            newEmployee.OrganizationId = newGuidId;

            var newUser = Mapper.Map<RegisterModel, User>(registerModel);
            newUser.PasswordHash = _jwtUtils.HashPassword(registerModel.Password);
            newUser.UserId = newGuidId;
            newUser.EmployeeId = newGuidId;
            newUser.OrganizationId = newGuidId;



            //await Validate<RegisterModel>(registerModel);
            //await Validate<Employee>(newEmployee);
            await Validate<Organization>(newUser.Organization);

            await ValidateUser(registerModel);

            if (Errors.Count > 0)
            {

                throw new MSException(System.Net.HttpStatusCode.BadRequest, Errors);
            }

            UnitOfWork.BeginTransaction();

            // Thêm cửa hàng mới:
            addOrgSuccess = await UnitOfWork.Organizations.AddAsync(newUser.Organization) > 0;

            // Thêm nhân viên mặc định:
            if (addOrgSuccess)
            {
                addEmployeeSuccess = await UnitOfWork.Employees.AddAsync(newEmployee) > 0;
            }

            // Thêm tài khoản để đăng nhập:
            if (addEmployeeSuccess)
            {
                addUserSuccess = await UnitOfWork.Users.Register(newUser) > 0;
            }

            // Cấp quyền để quản lý:
            if (addUserSuccess)
            {
                // Cấp quyền quản lý cho đơn vị/ đại lý/ cửa hàng đăng ký mới:
                var userRoleId = await UnitOfWork.UserRoles.GetUserRoleIdByRoleValue((int)MSEnums.MSRole.SuperManager);
                var userRole = new UserRole() { UserId = newUser.UserId, RoleId = userRoleId };
                addRoleSuccess = await UnitOfWork.UserRoles.AddAsync(userRole) > 0;
            }

            // Cấp được quyền là mọi thứ hoàn thành - trả về 1 cho nó oke
            // Nếu do Admin đăng ký thì không cần làm việc này, ngoài ra mặc định thông tin Email là được xác nhận:
            if (addRoleSuccess && registerModel.IsAdminRegister == false)
            {
                // Gửi Email xác nhận thành công:
                _emailService.SendMailTo = newUser.Email;
                var userTokenConfirm = _emailService.SentMailConfirmToken(newUser);

                // -> Nếu gửi được email thành công thì thực hiện lưu trữ Token trong Db để người dùng có thể xác nhận:
                
                if (userTokenConfirm != null)
                {
                    await UnitOfWork.UserTokenConfirms.AddAsync(userTokenConfirm);
                }
                UnitOfWork.Commit();

                // Cập nhật mã đơn vị:--> cần thì dùng cho bước xác nhận, khỏi cần truy cập Db
                CommonFunction.OrganizationId = newGuidId.ToString(); ;
                return 1;
            }
            else
            {
                throw new MSException(System.Net.HttpStatusCode.InternalServerError, "Có lỗi khi xử lý đăng ký tài khoản mới, vui lòng liên hệ Quản trị viên để được trợ giúp.");
            }

        }

        public async Task<AuthenticateResponse?> Authenticate(AuthenticateRequest model)
        {
            var user = await UnitOfWork.Users.GetUserAuthenticate(model.Username, _jwtUtils.HashPassword(model.Password));
            // return null if user not found
            if (user == null)
                return null;
            // Lấy thông tin đơn vị/ license:
            var licenseInfo = await UnitOfWork.MSLicenses.GetLicenseByUserId(user.UserId);

            user.LicenseExpiredDate = licenseInfo.ExpiredDate;

            // authentication successful so generate jwt token
            var token = _jwtUtils.GenerateJwtToken(user);

            return new AuthenticateResponse(user, token);
        }

        public IEnumerable<User> GetAll()
        {
            return null;
        }

        public async Task<UserInfo> GetUserInfoById(string id)
        {
            return await UnitOfWork.Users.GetUserInfoResponseById(id);
        }

        /// <summary>
        /// Kiểm tra các thông tin của người dùng đã hợp lệ hay chưa?
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        private async Task<bool> ValidateUser(RegisterModel user)
        {
            //if (await _repository.CheckEmailExist(user.Email) == true)
            //    Errors.Add("Email đã được đăng ký.");
            //if (await UnitOfWork.Users.CheckPhoneNumberExist(user.PhoneNumber) == true)
            //    AddErrors("PhoneNumber", $"Số điện thoại [{user.PhoneNumber}] đã được sử dụng bởi 1 thành viên khác.");
            if (user.Password.Trim() != user.RePassword.Trim())
                AddErrors("RePassword", "Mật khẩu xác nhận không khớp.");
            if (await UnitOfWork.Users.CheckUserNameExist(user.UserName) == true)
                AddErrors("UserName", $"Tài khoản đăng nhập [{user.UserName}] được sử dụng bởi 1 thành viên khác.");
            if (Errors.Count > 0)
            {
                return false;
            }
            return true;
        }

        public object? LogOut()
        {
            throw new NotImplementedException();
        }

        public async Task<int> ConfirmMultiUser(IEnumerable<User> users)
        {
            return await UnitOfWork.Users.ConfirmMultiUser(users);
        }

        public async Task<bool> ConfirmEmail(UserTokenConfirm user)
        {
            UnitOfWork.BeginTransaction();
            // Kiểm tra mã xác thực:
            var res = await UnitOfWork.Users.ConfirmUserToken(user);

            // Nếu xác thực thành công thì tặng thêm 15 ngày VIP:
            if (res == true)
            {
                var licenseCode = await UnitOfWork.MSLicenses.GetNewCodeDynamic();
                var expireDate = CommonFunction.ConvertDateToVNTime(DateTime.Now.AddDays(15));
                var startDate = CommonFunction.ConvertDateToVNTime(DateTime.Now);
                var license = new MSLicense()
                {
                    MSLicenseId = Guid.NewGuid(),
                    MSLicenseCode = licenseCode,
                    LicenseType = LicenseType.Trial,
                    StartDate = startDate,
                    ExpiredDate = expireDate
                };
                var org = await UnitOfWork.Organizations.GetOrganizationByUserName(user.UserName);
                license.OrganizationId = org?.OrganizationId;
                var addLicenseNumber = await UnitOfWork.MSLicenses.AddAsync(license);
                if (addLicenseNumber > 0)
                {
                    var emailTitle = $"[Mạnh Software] Thư thông báo xác nhận Email thành công";
                    var emailContent = $"Bạn đã xác nhận Email thành công cho [{org.OrganizationName}].<br>" +
                        $"Bạn sẽ nhận được quyền thành viên VIP trong 15 ngày. Bạn có thể gia hạn quyền VIP bất cứ lúc nào <br>" +
                        $"Liên hệ: Mr Mạnh (0961.17.9969) nếu bạn cần gia hạn hoặc trợ giúp! <br>" +
                        $"Cám ơn bạn đã sử dụng dịch vụ của Mạnh Software!";
                    _emailService.SentEmail(user.Email, emailTitle, emailContent);
                }
            }
            UnitOfWork.Commit();
            return res;
        }

        public async Task<bool> ChangePassword(UserChangePassword userChangePassword)
        {
            if (string.IsNullOrEmpty(userChangePassword.UserName))
            {
                AddErrors("UserName","Tên tài khoản không được phép để trống.");
            }
            if (string.IsNullOrEmpty(userChangePassword.Password))
            {
                AddErrors("Password", "Mật khẩu không được phép để trống.");
            }
            if (string.IsNullOrEmpty(userChangePassword.NewPassword))
            {
                AddErrors("NewPassword", "Mật khẩu mới không được phép để trống.");
            }
            if (userChangePassword.NewPassword != userChangePassword.ReNewPassword)
            {
                AddErrors("ReNewPasseword", "Xác nhận mật khẩu mới không đúng.");
            }
            if (Errors.Count > 0)
            {
                throw new MSException(System.Net.HttpStatusCode.BadRequest, Errors);
            }
            else
            {
                // Kiểm tra mật thông tin đăng nhập đã đúng hay chưa?
                userChangePassword.Password = _jwtUtils.HashPassword(userChangePassword.Password);
                var userInfo = await UnitOfWork.Users.GetUserAuthenticate(userChangePassword.UserName, userChangePassword.Password);

                // Nếu đúng thì cho đổi mật khẩu, sai thì báo lỗi:
                if (userInfo != null)
                {
                    var passwordHash = _jwtUtils.HashPassword(userChangePassword.NewPassword);
                    var res = await UnitOfWork.Users.ChangePassword(userInfo.UserId, passwordHash);
                    if (res)
                    {
                        // Gửi email đã đổi mật khẩu:
                        var email = userInfo.Email;
                        _emailService.SentEmail(email, "[Mạnh Software] Thông báo đổi mật khẩu",
                            $"Tài khoản <b>{userChangePassword.UserName}</b> đã thay đổi mật khẩu thành công!.<br>" +
                            $"Nếu không phải bạn đổi mật khẩu, hãy thông báo cho quản trị viên và sử dụng tính năng khôi phục tài khoản ngay lập tức.<br>" +
                            $"Trân trọng cám ơn!");
                    }
                    return res;
                }
                else
                {
                    throw new MSException(System.Net.HttpStatusCode.BadRequest,"Thông tin tài khoản hoặc mật khẩu không chính xác.");
                }
            }
        }
    }
}
