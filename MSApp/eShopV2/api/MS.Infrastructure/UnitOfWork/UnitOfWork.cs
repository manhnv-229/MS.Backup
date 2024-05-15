using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using MS.ApplicationCore.Interfaces;
using MySqlConnector;
using MS.ApplicationCore.Interfaces.Repositories;

namespace MS.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        #region Properties
        public IEmployeeRepository Employees { get; }
        public IPictureRepository Pictures { get; }
        public IUserRepository Users { get; }
        public IUserRolesRepository UserRoles { get; }
        public IExpenditurePlanRepository ExpenditurePlans { get; }
        public IExpenditureRepository Expenditures { get; }
        public IRoleRepository Roles { get; }
        public IProductRepository Products { get; }
        public IInvoiceDetailRepository InvoiceDetails { get; }
        public IInvoiceRepository Invoices { get; }
        public IOrganizationRepository Organizations { get; }
        public ICustomerGroupRepository CustomerGroups { get; }
        public IGroupProductRepository GroupProducts { get; }
        public IUnitRepository Units { get; }
        public ILicenseRepository MSLicenses { get; }
        public IUserTokenConfirmRepository UserTokenConfirms { get; }
        public ICustomerRepository Customers { get; }
        public IPositionRepository Positions { get; }
        #endregion

        private readonly MySqlDbContext _dbContext;
        Guid _id = Guid.Empty;
        public UnitOfWork(
            IPictureRepository pictures,
            IUserRepository users,
            MySqlDbContext dbContext,
            IUserRolesRepository userRoles,
            IExpenditurePlanRepository expenditurePlans,
            IExpenditureRepository expenditures,
            IEmployeeRepository employees,
            IRoleRepository roles,
            IProductRepository products,
            IInvoiceDetailRepository invoiceDetails,
            IInvoiceRepository invoices,
            IOrganizationRepository organizations,
            ICustomerGroupRepository customerGroups,
            IGroupProductRepository groupProducts,
            IUnitRepository units,
            ILicenseRepository mSLicenses,
            IUserTokenConfirmRepository userTokenConfirms,
            ICustomerRepository customers,
            IPositionRepository positions)
        {
            _id = Guid.NewGuid();
            Pictures = pictures;
            Users = users;
            _dbContext = dbContext;
            UserRoles = userRoles;
            ExpenditurePlans = expenditurePlans;
            Expenditures = expenditures;
            Employees = employees;
            Roles = roles;
            Products = products;
            InvoiceDetails = invoiceDetails;
            Invoices = invoices;
            Organizations = organizations;
            CustomerGroups = customerGroups;
            GroupProducts = groupProducts;
            Units = units;
            MSLicenses = mSLicenses;
            UserTokenConfirms = userTokenConfirms;
            Customers = customers;
            Positions = positions;
        }

        public Guid Id
        {
            get { return _id; }
        }

        public void BeginTransaction()
        {
            _dbContext.Transaction = _dbContext.Connection.BeginTransaction();
        }

        public void Commit()
        {
            _dbContext.Transaction.Commit();
        }

        public void Rollback()
        {
            _dbContext.Transaction.Rollback();
        }

        public void Dispose()
        {
            _dbContext.Transaction?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
