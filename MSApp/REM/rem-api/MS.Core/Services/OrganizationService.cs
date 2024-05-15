using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using MS.Core.Core;
using MS.Core.DTOs;
using MS.Core.Entities;
using MS.Core.Entities.Auth;
using MS.Core.Exceptions;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Caches;
using MS.Core.Interfaces.Repositories;
using MS.Core.Interfaces.Services;
using MS.Core.Interfaces.Shared;
using MS.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Services
{
    public class OrganizationService : BaseService<Organization>, IOrganizationService
    {
        ICommonFunction _commonFunction;
        private readonly IHubContext<NotificationHub> _notificationHub;
        IFileTransfer _fileTransfer;
        IEmailService _emailService;
        IMemoryCache _msMemoryCache;
        IShareData _shareData;
        bool? _isAdministrator;
        public OrganizationService(IOrganizationRepository repository, IUnitOfWork unitOfWork, IMapper mapper, IHubContext<NotificationHub> notificationHub, ICommonFunction commonFunction, IFileTransfer fileTransfer, IEmailService emailService, IMemoryCache msMemoryCache, IShareData shareData) : base(repository, unitOfWork, mapper)
        {
            _notificationHub = notificationHub;
            _commonFunction = commonFunction;
            _fileTransfer = fileTransfer;
            _emailService = emailService;
            _msMemoryCache = msMemoryCache;
            _shareData = shareData;
            // Nếu cha ở trạng thái xóa thì xóa hết cả các con -> không cần quan tâm trạng thái của con:
            // Chỉ cho phép Admin mới được xóa:
            var roles = _shareData?.User?.Roles;
            _isAdministrator = roles?.Any(r => r.RoleValue == MSEnums.MSRole.Administrator);

        }

        protected async override Task BeforeSave(dynamic entity)
        {
            if (entity is Organization)
            {
                var orgEntity = entity as Organization;
                var navbarItems = orgEntity.NavbarItem;

                if (navbarItems != null)
                {
                    foreach (var item in navbarItems)
                    {
                        if (item.EntityState == MSEnums.MSEntityState.DELETE)
                        {
                            if (_isAdministrator == false)
                            {
                                AddErrors("Forbidden", $"Bạn không được quyền xóa Menu [{item.Label}].");
                            }
                        }
                        else
                        {
                            if (item.NavbarSubItems.Any(i => i.EntityState != MSEnums.MSEntityState.DELETE))
                            {
                                item.HasSub = true;
                            }
                            // Chỉ cần có 1 phần tử bị thay đổi thì lập tức Update trạng thái cho State của cha để được thực hiện cập nhật.
                            if (item.NavbarSubItems.Any(i => i.EntityState != 0) && item.EntityState == 0)
                            {
                                item.EntityState = MSEnums.MSEntityState.UPDATE;
                            }
                        }

                        ProcessNavbarItem(item);
                    }
                }
            }
        }

        /// <summary>
        /// Xử lý trạng thái các menu item
        /// - Nếu xóa cha thì xóa toàn bộ các menu con - cần quyền Admin (tạm thời xử lý ở Repo)
        /// - Nếu update cha thì update toàn bộ con, cháu chắt - cần quyền Admin (tạm thời xử lý ở Repo)
        /// </summary>
        /// <param name="navItem"></param>
        private void ProcessNavbarItem(NavbarItem navItem)
        {
            var masterId = navItem.NavbarItemId;

            foreach (var item in navItem.NavbarSubItems)
            {

                if (navItem.EntityState == MSEnums.MSEntityState.DELETE)
                {
                    item.EntityState = MSEnums.MSEntityState.DELETE;
                    if (_isAdministrator == false)
                    {
                        AddErrors("Forbidden", $"Bạn không được quyền xóa Menu [{item.Label}].");
                    }
                    ProcessNavbarItem(item);
                }
                // Nếu cha không ở trạng thái xóa thì tùy thuộc trạng thái của con mà action tương ứng:
                else
                {
                    // Gán lại ParentId,:
                    item.ParentId = masterId;
                    if (item.EntityState == MSEnums.MSEntityState.DELETE && _isAdministrator == false)
                    {
                        AddErrors("Forbidden", $"Bạn không được quyền xóa Menu [{item.Label}].");
                    }
                    // Tiếp tục xử lý cho con, cháu, chắt nếu có:
                    if (item.NavbarSubItems.Count > 0)
                    {
                        ProcessNavbarItem(item);
                    }
                }
            }
        }

        protected async override Task AfterSave(dynamic entity, bool saveSuccess = true)
        {
            // Cập nhật lại cache:
            if (entity is Organization)
            {
                var orgEntity = entity as Organization;
                var currentSettings = orgEntity?.Setting;
                if (_msMemoryCache.TryGetValue("Settings", out IEnumerable<Setting> settings) && currentSettings != null)
                {
                    _msMemoryCache.Set("Settings", currentSettings);
                }
                else
                {
                    settings = await UnitOfWork.Settings.GetAllAsync();
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        //.SetSlidingExpiration(TimeSpan.FromSeconds(60))
                        //.SetAbsoluteExpiration(TimeSpan.FromSeconds(3600))
                        .SetPriority(CacheItemPriority.NeverRemove);
                    //.SetSize(1024);
                    _msMemoryCache.Set("Settings", settings, cacheEntryOptions);
                }
            }
        }
        public override async Task<PagingData> GetFilterPaging(string keyFilter, int limit, int offset, HashSet<ColumnSortInfo> columnSorts = null, HashSet<ColumnFilterInfo> columnFilters = null)
        {
            var colmnsSortString = new string[] { "OrganizationCode ASC" };
            var columnsFilterString = new string[] { "OrganizationName" };
            var data = await UnitOfWork.Organizations.GetFilterPaging(colmnsSortString, columnsFilterString, keyFilter, limit, offset);
            return data;
        }

        public async override Task<int> RemoveAsync(object key)
        {
            var currentOrgId = Guid.Parse(CommonFunction.OrganizationId);
            var orgID = Guid.Parse(key.ToString());
            if (currentOrgId == orgID)
            {
                throw new MSException(System.Net.HttpStatusCode.BadRequest, "Bạn không thể xóa công ty đang sở hữu!");
            }

            var userId = _commonFunction.GetCurrentUserId();
            var connections = NotificationHub._connections;
            UnitOfWork.BeginTransaction();
            // Xóa chi tiết hóa đơn:
            _ = await UnitOfWork.InvoiceDetails.RemoveByOrganizationAsync(orgID);
            await ShowDeleteProgress(connections, userId, "InvoiceDetails");

            // Xóa hóa đơn:
            _ = await UnitOfWork.Invoices.RemoveByOrganizationAsync(orgID);
            await ShowDeleteProgress(connections, userId, "Invoices");

            // Xóa sản phẩm:
            _ = await UnitOfWork.Products.RemoveByOrganizationAsync(orgID);
            await ShowDeleteProgress(connections, userId, "Products");

            // Xóa nhóm sản phẩm:
            _ = await UnitOfWork.GroupProducts.RemoveByOrganizationAsync(orgID);
            await ShowDeleteProgress(connections, userId, "GroupProducts");

            // Xóa đơn vị tính:
            _ = await UnitOfWork.Units.RemoveByOrganizationAsync(orgID);
            await ShowDeleteProgress(connections, userId, "Units");

            // Xóa khách hàng:
            _ = await UnitOfWork.Customers.RemoveByOrganizationAsync(orgID);
            await ShowDeleteProgress(connections, userId, "Customers");

            // Xóa nhóm khách hàng:
            _ = await UnitOfWork.CustomerGroups.RemoveByOrganizationAsync(orgID);
            await ShowDeleteProgress(connections, userId, "CustomerGroups");

            // Xóa vị trí: chức vụ:
            _ = await UnitOfWork.Positions.RemoveByOrganizationAsync(orgID);
            await ShowDeleteProgress(connections, userId, "Positions");

            // Xóa quyền:
            _ = await UnitOfWork.UserRoles.RemoveByOrganizationAsync(orgID);
            await ShowDeleteProgress(connections, userId, "UserRoles");

            // Xóa User:
            _ = await UnitOfWork.Users.RemoveByOrganizationAsync(orgID);
            await ShowDeleteProgress(connections, userId, "Users");

            // Xóa Nhân viên:
            _ = await UnitOfWork.Employees.RemoveByOrganizationAsync(orgID);
            await ShowDeleteProgress(connections, userId, "Employees");

            // Xóa License:
            _ = await UnitOfWork.MSLicenses.RemoveByOrganizationAsync(orgID);
            await ShowDeleteProgress(connections, userId, "MSLicenses");

            // Xóa đơn vị:
            _ = await UnitOfWork.Organizations.RemoveByOrganizationAsync(orgID);
            await ShowDeleteProgress(connections, userId, "Organizations", true);
            UnitOfWork.Commit();
            return 1;
        }

        private async Task ShowDeleteProgress(ConnectionMapping<string> connections, string userId, string progressName, bool isDone = false)
        {
            foreach (var connectionId in connections.GetConnections(userId))
            {
                await _notificationHub.Clients.Client(connectionId).SendAsync("RecieveRemoveOrganizationInfo", progressName, isDone);
                //await _notificationHub.Clients.All.SendAsync("RecieveRemoveOrganizationInfo");
            }
        }


        async Task<int> IOrganizationService.RenewLicense(MSLicenseRenew licenseInfo)
        {
            /// Kiểm tra hạn VIP hiện tại:
            /// Nếu đã hết hạn thì gia hạn mới tính từ ngày hiện tại.
            /// Nếu còn hạn thì cộng thêm:
            UnitOfWork.BeginTransaction();
            var res = 0;
            var currentLicense = await UnitOfWork.MSLicenses.GetLicenseByOrganizationId(licenseInfo.OrganizationId);
            var dateNow = CommonFunction.ConvertDateToVNTime(DateTime.Now);
            var expireDate = dateNow;
            if (currentLicense?.ExpiredDate != null)
            {
                expireDate = CommonFunction.ConvertDateToVNTime(currentLicense.ExpiredDate);
            }

            if (dateNow < expireDate)
            {
                currentLicense.ExpiredDate = expireDate.Value.AddDays((int)licenseInfo.VIPOption);
                res = await UnitOfWork.MSLicenses.UpdateAsync(currentLicense);
            }
            else
            {
                var newLicense = new MSLicense()
                {
                    MSLicenseId = Guid.NewGuid(),
                    MSLicenseCode = await UnitOfWork.MSLicenses.GetNewCodeDynamic(),
                    OrganizationId = licenseInfo.OrganizationId,
                    LicenseType = MSEnums.LicenseType.Genuine,
                    StartDate = dateNow,
                    ExpiredDate = dateNow.Value.AddDays((int)licenseInfo.VIPOption),
                    CreatedDate = dateNow,
                    CreatedBy = _commonFunction.GetCurrentUserId()
                };
                res = await UnitOfWork.MSLicenses.AddAsync(newLicense);
            }
            UnitOfWork.Commit();
            return res;
        }

        public async Task<int> DeleteLicense(Guid organizationId)
        {
            UnitOfWork.BeginTransaction();
            var res = await UnitOfWork.MSLicenses.DeleteLicenseByOrganizationId(organizationId);
            UnitOfWork.Commit();
            return res;
        }

        public async Task BackupDatabase()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            // the code that you want to measure comes here
            using MemoryStream ms = new();
            var fileBackup = UnitOfWork.Organizations.BackUpDatabase(ms);
            var timeStart = CommonFunction.ConvertDateToVNTime(DateTime.Now).ToString();
            var dateNowString = CommonFunction.ConvertDateToVNTime(DateTime.Now).ToString().Replace('/', '.').Replace(" ", "-");
            var dqlFileName = $"REM_{dateNowString}";
            var path = await _fileTransfer.UploadFileAsync(fileBackup, "database_backup", dqlFileName);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            if (!string.IsNullOrEmpty(path))
            {
                Console.WriteLine($"Backup success! file path: {path}");
                // Gửi email cho admin:
                var email = "manhnv229@gmail.com";
                var emailTitle = $"[shop.nmanh.com] Sao lưu dữ liệu ngày {timeStart}";
                var emailContent = $"Chào anh Mạnh! <br>" +
                    $"Dữ liệu của <b>shop.nmanh.com</b> đã được sao lưu thành công!" +
                    $"<li>Sao lưu lúc:{timeStart}</li>" +
                    $"<li>Thời gian thực hiện: {elapsedMs / 1000}s</li>" +
                    $"<li>Đường dẫn: {path}</li>" +
                    $"Mạnh Software trân trọng cám ơn anh! <br>";
                _emailService.SentEmail(email, emailTitle, emailContent);
            }
            else
            {
                Console.WriteLine("Backup database error!");
            }
        }

        public async Task<IEnumerable<OrganizationStatisticDto>> GetOrgStatisticByDay(DateTime date, bool sentEmail = false)
        {
            var dateVN = CommonFunction.ConvertDateToVNTime(date);
            var orgStatisticInfo = await UnitOfWork.Organizations.GetOrgStatisticByDay(dateVN);
            if (sentEmail)
            {
                var tableHeader = "" +
                    "<thead>" +
                        "<tr>" +
                            "<th style='padding: 6px 12px;'>STT</th>" +
                            "<th style='padding: 6px 12px;text-align: left;'>Đơn vị</th>" +
                            "<th style='padding: 6px 12px;text-align: right;'>Số lượng hóa đơn</th>" +
                            "<th style='padding: 6px 12px;text-align: right;'>Tổng thu nhập</th>" +
                        "</tr>" +
                    "</thead>";
                var tableRowStatistic = "";
                var sort = 1;
                foreach (var statistic in orgStatisticInfo)
                {
                    var email = statistic.Email;
                    var orgName = statistic.OrganizationName;
                    var totalMoneyString = string.Format("{0:n0}", statistic.TotalMoney);
                    var dateString = statistic.InvoiceDate.ToShortDateString();
                    var emailTitle = $"Thống kê thu nhập [{orgName}] ngày {dateString}: + {totalMoneyString}";
                    var emailContent = $"Chào [{orgName}]! <br>" +
                        $"Ngày ({dateString}), bạn đã tạo tổng số <b>{statistic.TotalInvoice}</b> hóa đơn, thu về <span style='color: green;'><b>{totalMoneyString} đ</b></span>. <br>" +
                        $"Nếu chưa nhập hết hóa đơn xin vui lòng nhập bổ sung, hệ thống sẽ cập nhật và lưu trữ đầy đủ cho bạn. <br/>" +
                        $"Mọi vấn đề cần hỗ trợ xin vui lòng liên hệ:" +
                        $"<li>SĐT: 0961.179.969</li>" +
                        $"<li>Email: manhnv229@gmail.com</li>" +
                        $"Mạnh Software trân trọng cám ơn bạn đã sử dụng dịch vụ! <br>" +
                        $"Lưu ý: ĐÂY LÀ EMAIL TỰ ĐỘNG ĐƯỢC GỬI TỪ HỆ THỐNG, QUÝ KHÁCH KHÔNG CẦN TRẢ LỜI EMAIL NÀY!";

                    tableRowStatistic += "<tr>" +
                                            $"<td style='padding: 6px 12px;'>{sort}</td>" +
                                            $"<td style='padding: 6px 12px;text-align: left'>{statistic.OrganizationName}</td>" +
                                            $"<td style='padding: 6px 12px;text-align: right'>{statistic.TotalInvoice}</td>" +
                                            $"<td style='padding: 6px 12px;text-align: right'>{totalMoneyString} đ</td>" +
                                        $"</tr>";
                    _emailService.SentEmail(email, emailTitle, emailContent);
                    sort++;
                }

                // Gửi Email cho QTV:
                var styleCSS = "<style>" +
                    "td{padding: 0 12px;}</style>";
                var tableHTML = "<table border='1' style = 'border-collapse: collapse;  margin: 10px;'>" +
                                        $"{tableHeader}" +
                                        $"<tbody>{tableRowStatistic}</tbody>" +
                                     $"</table>";
                var emailToAdminContent = $"{styleCSS}" +
                    "Chào anh Mạnh,<br>" +
                    $"Em gửi anh chi tiết thống kê hoạt động các đơn vị ngày <b>{date.ToShortDateString()}</b>: <br>" +
                    $"{tableHTML}" +
                    $"Anh vui lòng kiểm tra kỹ, nếu có vấn đề vui lòng kiểm tra và sửa lỗi!<br>" +
                    $"Trân trọng cám ơn anh!";
                _emailService.SentEmail("manhnv229@gmail.com", $"[shop.nmanh.com] THỐNG KÊ HOẠT ĐỘNG {date.ToShortDateString()}", emailToAdminContent);
            }
            return orgStatisticInfo;
        }
    }
}
