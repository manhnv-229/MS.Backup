using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using MS.Core.Core;
using MS.Core.DTOs;
using MS.Core.Entities;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Repositories;
using MS.Core.Interfaces.Services;
using MS.Core.Interfaces.Shared;
using MS.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;
using Microsoft.AspNetCore.Hosting;
using static System.Net.WebRequestMethods;
using Microsoft.Extensions.Configuration;
using System.Windows.Input;
using System.Net.Http.Json;
using Newtonsoft.Json;
using MS.Core.Exceptions;
using System.Net.WebSockets;
using MS.Core.Authorization;
using MS.Core.Entities.Auth;
using MS.Core.MSEnums;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.RegularExpressions;

namespace MS.Core.Services
{
    public class TenantService : BaseService<Tenant>, ITenantService
    {
        readonly IEmailService _emailService;
        readonly IWebServerConfig _webServerConfig;
        IJwtUtils _jwtUtils;
        ICloudflareApi _cloudFlareApi;
        IDatabaseHelper _dbHelper;
        IHttpClientFactory _httpClientFactory;
        IServiceProvider _serviceProvider;
        readonly IConfiguration _config;
        readonly string _ssHost;
        readonly string _sshUserName;
        readonly string _sshPassword;
        readonly string _webFolderPath;
        readonly string _webDomainConfig;

        //Cloudframe
        readonly string _cloudflareClientApi;
        readonly string _cloudflareDNSApiToken;

        private IHostingEnvironment _env;
        ITenantRepository _repository;
        ICommonFunction _commonFunction;
        IUnitOfWork _uow;
        private readonly IHubContext<NotificationHub> _notificationHub;
        IMapper _mapper;
        IShareData _shareData;
        int _stepNumber = 0;
        public TenantService(IUnitOfWork unitOfWork,
            IMapper mapper,
            ITenantRepository repository,
            ICommonFunction commonFunction,
            IUnitOfWork uow,
            IHubContext<NotificationHub> notificationHub,
            IShareData shareData,
            IHostingEnvironment env,
            IConfiguration config,
            IServiceProvider serviceProvider,
            ICloudflareApi cloudFlareApi,
            IDatabaseHelper dbHelper,
            IJwtUtils jwtUtils,
            IEmailService emailService,
            IWebServerConfig webServerConfig) : base(repository, unitOfWork, mapper)
        {
            _repository = repository;
            _commonFunction = commonFunction;
            _uow = uow;
            _notificationHub = notificationHub;
            _shareData = shareData;
            _env = env;
            _config = config;
            _ssHost = _config["WebServer:Host"];
            _sshUserName = _config["WebServer:UserName"];
            _sshPassword = _config["WebServer:Password"];
            _webFolderPath = _config["WebServer:WebFolder:baza"];
            _webDomainConfig = _config["WebServer:WebDomain:baza"];

            _cloudflareClientApi = _config["Cloudflare:ClientApi"];
            _cloudflareDNSApiToken = _config["Cloudflare:DNSApiToken"];
            _httpClientFactory = serviceProvider.GetService(typeof(IHttpClientFactory)) as IHttpClientFactory;
            _serviceProvider = serviceProvider;
            _cloudFlareApi = cloudFlareApi;
            _dbHelper = dbHelper;
            _jwtUtils = jwtUtils;
            _emailService = emailService;
            _webServerConfig = webServerConfig;
        }

        public override async Task<PagingData> GetFilterPaging(string keyFilter, int limit, int offset, HashSet<ColumnSortInfo> columnSorts = null, HashSet<ColumnFilterInfo> columnFilters = null)
        {
            var colmnsSortString = new string[] { "CreatedDate DESC", "TenantCode ASC" };
            var columnsFilterString = new string[] { "TenantCode", "TenantName", "OrganizationName", "SubDomain", "RootDomain", "OrganizationPhoneNumber" };
            var data = await UnitOfWork.Tenants.GetFilterPaging(colmnsSortString, columnsFilterString, keyFilter, limit, offset);
            return data;
        }

        public async override Task<int> RemoveAsync(object key)
        {
            // Lấy thông tin Tenant:
            UnitOfWork.BeginTransaction();
            _stepNumber = 0;
            var tenant = await UnitOfWork.Tenants.GetByIdAsync(key);
            // Thực hiện kiểm tra phát sinh dữ liệu: (phát sinh hoá đơn, dịch vụ...)
            // Nếu chưa có nghiệp vụ phát sinh thì xoá Database:
            if (tenant?.Catalog != null)
            {
                var hasArising = await _dbHelper.CheckArisingBussinessDataAsync(tenant.Catalog);
                if (hasArising)
                {
                    await CallClientWhenProgressFail("CheckArisingBussinessDataFail");
                    throw new MSException(System.Net.HttpStatusCode.BadRequest, "Không thể xoá thuê bao này do đã có phát sinh dữ liệu và nghiệp vụ mua hàng trên ứng dụng.");
                }
                else
                {
                    await CallClientUpdateProgress("CheckArisingBussinessDataSuccess");

                    await _dbHelper.DropDatabase(tenant.Catalog);
                    await CallClientUpdateProgress("DropDatabaseSuccess");

                    // Xoá cấu hình Website:
                    var domain = $"{tenant.Catalog.SubDomain}.{tenant.Catalog.RootDomain}";
                    _webServerConfig.RemoveDomain(domain);
                    await CallClientUpdateProgress("RemoveDomainSuccess");

                    // Xoá DNS Record trên cloudflare:
                    var zone = await _cloudFlareApi.GetZoneByDomainAsync(tenant.Catalog.RootDomain);
                    if (zone != null)
                    {
                        var dnsRecord = await _cloudFlareApi.GetDNSRecordByName(zone.id, domain);
                        if (dnsRecord != null)
                        {
                            await _cloudFlareApi.DeleteDNSRecordById(zone.id, dnsRecord.id);
                        }

                    }
                    await CallClientUpdateProgress("DeleteDNSRecordSuccess");

                }
            }

            // Xoá thông tin thuê bao:
            var res = await UnitOfWork.Tenants.RemoveAsync(key);
            await CallClientUpdateProgress("RemoveTenatSuccess");
            UnitOfWork.Commit();

            Console.WriteLine($"Deleted: {tenant?.Catalog?.SubDomain}.{tenant?.Catalog?.RootDomain}");
            return res;
        }

        public async Task RegisterAsync(TenantRegister tenant)
        {
            try
            {
                tenant.EntityState = MSEntityState.ADD;
                tenant.Organization.EntityState = MSEntityState.ADD;
                tenant.Catalog.EntityState = MSEntityState.ADD;
                tenant.TenantUser.EntityState = MSEntityState.ADD;
                tenant.License.EntityState = MSEntityState.ADD;
                tenant.PhoneNumber = tenant.TenantUser.PhoneNumber;

                // Mở transaction:
                UnitOfWork.BeginTransaction();

                var newTenantId = Guid.NewGuid();

                // Thực hiện validate:
                await ValidateTenant(tenant);

                // Hashpassword:
                tenant.TenantUser.PasswordHash = _jwtUtils.HashPassword(tenant.TenantUser.Password);

                // Thực hiện thêm mới vào database:
                tenant.TenantId = newTenantId;
                var addTenant = await UnitOfWork.Tenants.AddAsync(tenant);
                if (addTenant > 0)
                {
                    Console.WriteLine("add tenant success.");
                    await CallClientUpdateProgress("AddTenantSuccess");

                    // Thêm thông tin đơn vị:
                    tenant.Organization.TenantId = newTenantId;
                    tenant.Organization.OrganizationId = Guid.NewGuid();
                    tenant.Organization.EntityState = MSEnums.MSEntityState.ADD;
                    var addOrg = await UnitOfWork.Organizations.AddAsync(tenant.Organization);

                    // Thêm thông tin CSDL của thuê bao:
                    tenant.Catalog.TenantId = newTenantId;
                    tenant.Catalog.EntityState = MSEnums.MSEntityState.ADD;
                    var addCatalog = await UnitOfWork.Catalogs.AddAsync(tenant.Catalog);

                    // Thêm thông tin User của thuê bao:
                    tenant.TenantUser.OrganizationId = tenant.Organization.OrganizationId;
                    tenant.TenantUser.UserId = Guid.NewGuid();
                    tenant.TenantUser.TenantId = newTenantId;
                    tenant.TenantUser.EntityState = MSEnums.MSEntityState.ADD;
                    var addTenantUser = await UnitOfWork.TenantUsers.AddAsync(tenant.TenantUser);

                    if (addOrg > 0 && addCatalog > 0 && addTenantUser > 0)
                    {
                        await CallClientUpdateProgress("AddOrgCatalogSuccess");
                    }
                };

                // Tạo database:
                await ConfigDatabase(tenant);

                // Xử lý cấu hình website:
                await ConfigWebsite(tenant);

                // Cấp License:
                await AddLicenseForNewRegister(tenant);

                // Gửi mail cho thuê bao đăng ký
                await SentEmailAfterRegisterSuccess(tenant);

                // Commit transaction:
                UnitOfWork.Commit();
            }
            catch (MSException ex)
            {
                await CallClientWhenProgressFail("ErrorException");
                throw;
            }
            catch (Exception ex)
            {
                await CallClientWhenProgressFail("ErrorException");
                throw new MSException(System.Net.HttpStatusCode.InternalServerError, "Có lỗi xảy ra trong quá trình thực hiện đăng ký.<br/> Vui lòng thử lại hoặc liên hệ QTV để được trợ giúp.");
            }

        }

        /// <summary>
        /// Thêm license khi đăng ký mới
        /// </summary>
        /// <param name="tenant"></param>
        /// <returns></returns>
        private async Task AddLicenseForNewRegister(TenantRegister tenant)
        {
            var licenseCode = CommonFunction.GetCurrentDateTimeString();
            var expireDate = CommonFunction.ConvertDateToVNTime(DateTime.Now.AddDays(30));
            var startDate = CommonFunction.ConvertDateToVNTime(DateTime.Now);
            var license = new MSLicense()
            {
                MSLicenseId = Guid.NewGuid(),
                MSLicenseCode = licenseCode,
                LicenseType = LicenseType.Trial,
                StartDate = startDate,
                ExpiredDate = expireDate
            };
            license.OrganizationId = tenant.Organization.OrganizationId;
            tenant.License = license;
            var res = await UnitOfWork.MSLicenses.AddAsync(license);
            await _dbHelper.AddFirstLicense(tenant);
            if (res > 0)
            {
                await CallClientUpdateProgress("AddLicenseForNewRegisterSuccess");
            }
        }

        /// <summary>
        /// Kiểm tra các thông tin thuê bao khi đăng ký mới
        /// </summary>
        /// <param name="tenant"></param>
        /// <returns></returns>
        /// <exception cref="MSException"></exception>
        private async Task ValidateTenant(TenantRegister tenant)
        {
            if (tenant == null)
            {
                throw new MSException(System.Net.HttpStatusCode.BadRequest, "Thông tin thuê bao không được trống.");
            }
            else
            {
                var isDuplicateTenantCode = await UnitOfWork.Tenants.CheckDuplicate(tenant.TenantCode, "TenantCode", tenant);
                if (isDuplicateTenantCode)
                {
                    AddErrors("TenantCode", "Mã thuê bao đã được sử dụng.");
                }

                var isDuplicatePhoneNumber = await UnitOfWork.Tenants.CheckDuplicate(tenant.PhoneNumber, "PhoneNumber", tenant);
                if (isDuplicatePhoneNumber)
                {
                    AddErrors("PhoneNumber", "Số điện thoại thuê bao đã được đăng ký.");
                }

                // Kiểm tra thông tin đơn vị:
                if (tenant.Organization == null)
                {
                    AddErrors("Organization", "Thông tin cửa hàng không được trống.");
                }

                // Kiểm tra thông tin user:
                if (tenant.TenantUser == null)
                {
                    AddErrors("User", "Thiếu thông tin tài khoản sử dụng ứng dụng.");
                }

                // Kiểm tra tài khoản có ký tự đặc biệt hay không
                if (CheckStringHasSpecialCharacters(tenant.TenantUser.UserName)) {
                    AddErrors("TenantUser.UserName", "Tài khoản không được chứa các ký tự đặc biệt.");
                }

                // Kiểm tra mật khẩu:
                if (tenant.TenantUser.Password != tenant.TenantUser.RePassword)
                {
                    AddErrors("TenantUser.Password", "Mật khẩu xác nhận không khớp.");
                }
                if (Errors.Count > 0)
                {
                    throw new MSException(System.Net.HttpStatusCode.BadRequest, Errors);
                }
                else
                {
                    await CallClientUpdateProgress("ValidateTenant");
                }
            }
        }

        /// <summary>
        /// Thực hiện cấu hình database khi đăng ký mới
        /// </summary>
        /// <param name="catalog"></param>
        /// <returns></returns>
        private async Task ConfigDatabase(TenantRegister tenant)
        {
            await _dbHelper.InitDbBAZA(tenant);
            await CallClientUpdateProgress("InitDbBAZASuccess");

            await _dbHelper.CreateFirstUserAndRole(tenant);
            await CallClientUpdateProgress("CreateFirstUserAndRoleSuccess");
        }


        /// <summary>
        /// Thực hiện cấu hình website:
        /// </summary>
        /// <param name="tenant"></param>
        /// <returns></returns>
        private async Task ConfigWebsite(TenantRegister tenant)
        {
            try
            {
                ConfigDomainToNginxServer(tenant);
                await CallClientUpdateProgress("ConfigDomainToNginxServerSuccess");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new MSException(ex.Message);
            }
            

            await ConfigDNSRecordInCloudflareServer(tenant);
            await CallClientUpdateProgress("ConfigDNSRecordInCloudflareServerSuccess");

        }


        /// <summary>
        /// Hàm thực hiện cấu hình sub domain và domain lên server web (nginx)
        /// </summary>
        /// <param name="sshClient"></param>
        /// <param name="tenant"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// CreatedBy: NVMANH (25/04/2024)
        private void ConfigDomainToNginxServer(TenantRegister tenant)
        {
            var rootDomain = tenant.Catalog.RootDomain;
            if (rootDomain == null)
                rootDomain = _webDomainConfig;
            var domain = $"{tenant.Catalog.SubDomain}.{rootDomain}";
            _webServerConfig.AddDomain(domain);
        }

        /// <summary>
        /// Thực hiện cấu hình thông tin DNS trên Cloudflare
        /// </summary>
        /// <param name="tenant"></param>
        /// <returns></returns>
        /// CreatedBy: NVMANH (25/04/2024)
        private async Task ConfigDNSRecordInCloudflareServer(TenantRegister tenant)
        {
            var rootDomain = tenant.Catalog.RootDomain;
            if (rootDomain == null)
                rootDomain = _webDomainConfig;

            var zone = await _cloudFlareApi.GetZoneByDomainAsync(rootDomain);
            var zoneId = zone?.id;
            var fullDomain = $"{tenant.Catalog.SubDomain}.{rootDomain}";
            if (zoneId != null)
            {
                // Kiểm tra record với subdomain đã có chưa- có thì sửa, chưa có thì thêm:
                var dnsRecordExits = await _cloudFlareApi.GetDNSRecordByName(zoneId, fullDomain);
                if (dnsRecordExits != null)
                {
                    dnsRecordExits.content = tenant.Catalog.Server;
                    dnsRecordExits.comment = $"last update  {DateTime.Now.ToString()}";
                    await _cloudFlareApi.UpdateDNSRecord(zoneId, dnsRecordExits);
                }
                else
                {
                    var dns = new CloudflareDNSRecord()
                    {
                        name = tenant.Catalog.SubDomain,
                        content = tenant.Catalog.Server,
                        comment = $"add from app {DateTime.Now.ToString()}",
                        proxied = true,
                        //ttl = 0,
                        type = "A"

                    };
                    await _cloudFlareApi.AddNewDNSRecord(dns, zoneId);
                }

            }
        }


        private bool CheckStringHasSpecialCharacters(string str)
        {
            var regexItem = new Regex("^[a-zA-Z0-9]*$");

            if (regexItem.IsMatch(str)) {
                return false;
            }
            else
            {
                return true;
            }
        }
        private async Task SentEmailAfterRegisterSuccess(TenantRegister tenant)
        {
            var org = tenant.Organization;
            var user = tenant.TenantUser;
            var emailTitle = $"[Mạnh Hào Software] Thư thông báo đăng ký thành công";
            var emailContent = $"Email này đã được sử dụng để đăng ký cho [{org.OrganizationName}].<br>" +
                $"Bạn sẽ nhận được quyền thành viên VIP trong 30 ngày. Bạn có thể gia hạn quyền VIP bất cứ lúc nào <br>" +
                $"Liên hệ: Mr Mạnh (0933.365.999) nếu bạn cần gia hạn hoặc trợ giúp! <br>" +
                $"Cám ơn bạn đã sử dụng dịch vụ của Mạnh Software!";
            _emailService.SentEmail(user.Email, emailTitle, emailContent);
            await CallClientUpdateProgress("SentEmailAfterRegisterSuccess");
        }
        private async Task CallClientUpdateProgress(string progressStepName)
        {
            if (_shareData.ConnectionId != null)
            {
                _stepNumber++;
                await _notificationHub.Clients.Client(_shareData.ConnectionId).SendAsync($"UpdateProgress", progressStepName, _stepNumber);

            }
        }

        private async Task CallClientWhenProgressFail(string progressStepName)
        {
            if (_shareData.ConnectionId != null)
            {
                _stepNumber++;
                await _notificationHub.Clients.Client(_shareData.ConnectionId).SendAsync($"UpdateProgressWhenHasError", progressStepName, _stepNumber);

            }
        }
    }
}
