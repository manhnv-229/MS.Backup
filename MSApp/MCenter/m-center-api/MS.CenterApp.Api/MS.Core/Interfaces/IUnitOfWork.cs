using MS.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace MS.Core.Interfaces
{
    public interface IUnitOfWork: IDisposable
    {
        Guid Id { get; }
        //IDbConnection Connection { get; }
        //IDbTransaction Transaction { get; }
        void BeginTransaction();
        void Commit();
        void Rollback();
        IPictureRepository Pictures { get; }
        IUserRepository Users { get; }
        IUserRolesRepository UserRoles { get; }
        IExpenditurePlanRepository ExpenditurePlans { get; }
        IExpenditureRepository Expenditures { get; }
        IEmployeeRepository Employees { get; }
        ICustomerRepository Customers { get; }
        IRoleRepository Roles { get; }
        IPositionRepository Positions { get; }
        IProductRepository Products { get; }
        IInvoiceRepository Invoices { get; }
        IInvoiceDetailRepository InvoiceDetails { get; }
        IOrganizationRepository Organizations { get; }
        ICustomerGroupRepository CustomerGroups { get; }
        IGroupProductRepository GroupProducts { get; }
        IUnitRepository Units { get; }
        ILicenseRepository MSLicenses { get; }
        IUserTokenConfirmRepository UserTokenConfirms { get; }

        IInventoryItemRepository InventoryItems { get; }

        IInventoryItemCategoryRepository InventoryItemCategories { get; }

        IRefDetailRepository RefDetails { get; }
        IRefRepository Refs { get; }
        ISettingRepository Settings { get; }
        IReportRepository Reports { get; }
        IStockInventoryRepository StockInventories { get; }
        INoteRepository Notes { get; }

        ISlotRepository Slots { get; }
        ISlotGroupRepository SlotGroups { get; }
        IReservationDetailRepository ReservationDetails { get; }
        IReservationRepository Reservations { get; }
        IServiceInvoiceRepository ServiceInvoices { get; }
        IServiceRepository Services { get; }
        IServiceGroupRepository ServiceGroups { get; }
        IOrderRepository Orders { get; }
        IBranchRepository Branches { get; }
        INavbarItemRepository NavbarItems { get; }
        ITenantRepository Tenants { get; }
        ITenantUserRepository TenantUsers { get; }
        ISystemConfigRepository SystemConfigs { get; }
        ICatalogRepository Catalogs { get; }
        IMSAppRepository MSApps { get; }
    }
}
