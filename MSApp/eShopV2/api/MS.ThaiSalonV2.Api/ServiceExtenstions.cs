using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MS.ApplicationCore;
using MS.ApplicationCore.Authorization;
using MS.ApplicationCore.Interface.Service;
using MS.ApplicationCore.Interfaces;
using MS.ApplicationCore.Interfaces.Repositories;
using MS.ApplicationCore.Interfaces.Services;
using MS.ApplicationCore.Services;
using MS.ApplicationCore.Utilities;
using MS.Infrastructure;
using MS.Infrastructure.Data;
using MS.Infrastructure.UnitOfWork;
using MySqlConnector;
using System.Data;
using System.Globalization;

namespace MS.ThaiSalonV2.Api
{
    public static class ServiceExtenstions
    {
        public static void AddApplication(this IServiceCollection service)
        {
            service.AddApplicationCore();
            //service.AddTransient<IUnitOfWork, UnitOfWork>(); //Chú ý sử dụng AddTransient
            service.AddScoped<MySqlDbContext>();// Chú ý phải sử dụng AddScoped để đảm bảo không mở nhiều kết nối (các kết nối được tạo ra sẽ được duy trì và sử dụng lại)
            //service.AddScoped<IDbConnection>(db => new MySqlConnection(""));
            service.AddTransient<IUnitOfWork, UnitOfWork>();
            service.AddTransient(typeof(IRepository<>), typeof(DapperRepository<>));
            service.AddTransient(typeof(IAsyncRepository<>), typeof(DapperRepository<>));
            service.AddTransient(typeof(IBaseService<>), typeof(BaseService<>));
            //service.AddScoped(typeof(IAsyncService<>), typeof(BaseService<>));
            
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
            service.AddHttpContextAccessor();


        }
    }
}
