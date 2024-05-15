using MS.ApplicationCore.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace MS.ApplicationCore.Interfaces
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
    }
}
