using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MISA.FM.Infrastructure.Repositories;
using MS.Api.Controllers;
using MS.Core;
using MS.Core.Authorization;
using MS.Core.Entities;
using MS.Core.Interface.Service;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Caches;
using MS.Core.Interfaces.Repositories;
using MS.Core.Interfaces.Services;
using MS.Core.Interfaces.Shared;
using MS.Core.Services;
using MS.Core.Shared;
using MS.Core.Utilities;
using MS.Infrastructure;
using MS.Infrastructure._3Party;
using MS.Infrastructure.Cache;
using MS.Infrastructure.Data;
using MS.Infrastructure.Interfaces;
using MS.Infrastructure.Storage;
using MS.Infrastructure.UnitOfWork;
using MySqlConnector;
using System.Data;
using System.Globalization;

namespace MS.CenterApp.Api
{
    public static class ServiceExtenstions
    {
        public static void AddApplication(this IServiceCollection service)
        {
            service.AddMemoryCache();
            service.AddApplicationCore();
            //service.AddTransient<IUnitOfWork, UnitOfWork>(); //Chú ý sử dụng AddTransient
            //service.AddScoped<MariaDbContext>();
            service.AddScoped<IMSDatabaseContext, MariaDbContext>();// Chú ý phải sử dụng AddScoped để đảm bảo không mở nhiều kết nối (các kết nối được tạo ra sẽ được duy trì và sử dụng lại)
            //service.AddScoped<IMSDatabaseContext, MariaDbContextDEF>();
            //service.AddScoped<IDbConnection>(db => new MySqlConnection(""));
            service.AddTransient<IUnitOfWork, UnitOfWork>();
            service.AddTransient(typeof(IRepository<>), typeof(DapperRepository<>));
            service.AddTransient(typeof(IAsyncRepository<>), typeof(DapperRepository<>));
            service.AddTransient(typeof(IBaseService<>), typeof(BaseService<>));
            //service.AddScoped(typeof(IAsyncService<>), typeof(BaseService<>));
            service.AddScoped<IShareData, SharedData>();

            service.AddTransient<IPictureService, PictureService>();
            service.AddTransient<IPictureRepository, PictureRepository>();
            service.AddTransient<IUserService, UserService>();
            service.AddTransient<IUserRepository, UserRepository>();
            service.AddTransient<IUserRolesRepository, UserRolesRepository>();
            service.AddTransient<ICommonFunction, CommonFunction>();
            service.AddTransient<IExpenditureRepository, ExpenditureRepository>();
            service.AddTransient<IExpenditureService, ExpenditureService>();
            service.AddTransient<IExpenditurePlanRepository, ExpenditurePlanRepository>();
            service.AddTransient<IExpenditurePlanService, ExpenditurePlanService>();
            service.AddTransient<IUserTokenConfirmRepository, UserTokenConfirmRepository>();

            // Danh mục enum chung
            service.AddTransient<IDictionaryEnumService, DictionaryEnumService>();

            // Khách hàng:
            service.AddTransient<ICustomerRepository, CustomerRepository>();
            service.AddTransient<ICustomerService, CustomerService>();

            // Nhân viên
            service.AddTransient<IEmployeeRepository, EmployeeRepository>();
            service.AddTransient<IEmployeeService, EmployeeService>();

            // vị trí
            service.AddTransient<IPositionRepository, PositionRepository>();
            service.AddTransient<IPositionService, PositionService>();

            service.AddTransient<IRoleRepository, RoleRepository>();

            //Sản phẩm
            service.AddTransient<IProductRepository, ProductRepository>();
            service.AddTransient<IProductService, ProductService>();

            //Hóa đơn
            service.AddTransient<IInvoiceService, InvoiceService>();
            service.AddTransient<IInvoiceRepository, InvoiceRepository>();

            //Chi tiết hóa đơn
            service.AddTransient<IInvoiceDetailRepository, InvoiceDetailRepository>();
            service.AddTransient<IInvoiceDetailService, InvoiceDetailService>();

            //Nhóm khách hàng
            service.AddTransient<ICustomerGroupService, CustomerGroupService>();
            service.AddTransient<ICustomerGroupRepository, CustomerGroupRepository>();

            //Nhóm sản phẩm
            service.AddTransient<IGroupProductService, GroupProductService>();
            service.AddTransient<IGroupProductRepository, GroupProductRepostitory>();

            // Nhóm hàng hoá:
            service.AddTransient<IInventoryItemCategoryService, InventoryItemCategoryService>();
            service.AddTransient<IInventoryItemCategoryRepository, InventoryItemCategoryRepository>();

            // Hàng hoá:
            service.AddTransient<IInventoryItemService, InventoryItemService>();
            service.AddTransient<IInventoryItemRepository, InventoryItemRepository>();

            // Hàng hoá:
            service.AddTransient<IRefService, RefService>();
            service.AddTransient<IRefRepository, RefRepository>();
            service.AddTransient<IRefDetailService, RefDetailService>();
            service.AddTransient<IRefDetailRepository, RefDetailRepository>();

            //Đơn vị tính
            service.AddTransient<IUnitService, UnitService>();
            service.AddTransient<IUnitRepository, UnitRepostitory>();

            //Đơn vị
            service.AddTransient<IOrganizationService, OrganizationService>();
            service.AddTransient<IOrganizationRepository, OrganizationRepostiory>();

            // Giấy phép sử dụng:
            service.AddTransient<ILicenseRepository, MSLicenseRepository>();
            service.AddTransient<ILicenseService, MSLicenseService>();

            service.AddTransient<IJwtUtils, JwtUtils>();
            service.AddTransient<IEmailService, EmailService>();
            service.AddTransient<IFileTransfer, FileTransfer>();
            service.AddTransient<ISettingService, SettingService>();
            service.AddTransient<ISettingRepository, SettingRepository>();
            service.AddSingleton<MSMemoryCache>();
            service.AddHttpContextAccessor();

            service.AddTransient<IReportService, ReportService>();
            service.AddTransient<IReportRepository, ReportRepository>();

            service.AddTransient<IStockInventoryService, StockInventoryService>();
            service.AddTransient<IStockInventoryRepository, StockInventoryRepository>();

            service.AddTransient<INoteService, NoteService>();
            service.AddTransient<INoteRepository, NoteRepository>();

            service.AddTransient<ISlotService, SlotService>();
            service.AddTransient<ISlotRepository, SlotRepository>();

            service.AddTransient<ISlotGroupService, SlotGroupService>();
            service.AddTransient<ISlotGroupRepository, SlotGroupRepository>();

            // Đặt chỗ:
            service.AddTransient<IReservationService, ReservationService>();
            service.AddTransient<IReservationRepository, ReservationRepostitory>();
            service.AddTransient<IReservationDetailService, ReservationDetailService>();
            service.AddTransient<IReservationDetailRepository, ReservationDetailRepostitory>();

            // Order:
            service.AddTransient<IOrderService, OrderService>();
            service.AddTransient<IOrderRepository, OrderRepository>();
            service.AddTransient<IServiceInvoiceRepository, ServiceInvoiceRepository>();
            service.AddTransient<ISlotInvoiceRepository, SlotInvoiceRepository>();

            //Service 
            service.AddTransient<IServiceService, ServiceService>();
            service.AddTransient<IServiceRepository, ServiceRepository>();
            service.AddTransient<IServiceGroupService, ServiceGroupService>();
            service.AddTransient<IServiceGroupRepository, ServiceGroupRepository>();

            service.AddTransient<IBranchService, BrachService>();
            service.AddTransient<IBranchRepository, BranchRepository>();

            service.AddTransient<INavbarItemRepository, NavbarItemRepository>();

            service.AddTransient<ITenantService, TenantService>();
            service.AddTransient<ITenantRepository, TenantRepository>();
            service.AddTransient<ITenantUserRepository, TenantUserRepository>();

            service.AddTransient<ICloudflareApi, CloudFlare>();
            service.AddTransient<IDatabaseHelper, MariaDbHelper>();

            service.AddTransient<ISystemConfigService, SystemConfigService>();
            service.AddTransient<ISystemConfigRepository, SystemConfigRepository>();

            service.AddTransient<ICatalogRepository, CatalogRepository>();

            service.AddTransient<IWebServerConfig, UbuntuNginxWebConfig>();

            service.AddTransient<IMSAppRepository, MSAppRepository>();

            service.AddHttpContextAccessor();
            //   System.Reflection.Assembly.GetExecutingAssembly()
            //.GetTypes()
            //.Where(item => item.GetInterfaces()
            //.Where(i => i.IsGenericType).Any(i => i.GetGenericTypeDefinition() == typeof(IDatabaseService<>)) && !item.IsAbstract && !item.IsInterface)
            //.ToList()
            //.ForEach(assignedTypes =>
            //{
            //    var serviceType = assignedTypes.GetInterfaces().First(i => i.GetGenericTypeDefinition() == typeof(IDatabaseService<>));
            //    services.AddScoped(serviceType, assignedTypes);
            //});
        }
    }
}
