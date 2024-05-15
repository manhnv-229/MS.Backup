using Dapper;
using MS.Core.Entities;
using MS.Core.Interfaces.Repositories;
using MS.Core.Utilities;
using MS.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Infrastructure.Data
{
    public class ReportRepository : DapperRepository<Report>, IReportRepository
    {
        public ReportRepository(IMSDatabaseContext dbContext) : base(dbContext)
        {
        }

        public async Task<object> GetProfitAmountByItems(DateTime? startDate, DateTime? endDate, string categoryId)
        {
            var storeName = "Proc_GetProfitAmountByItems";
            var parameters = new DynamicParameters();
            parameters.Add("@p_startDate", startDate);
            parameters.Add("@p_endDate", endDate);
            parameters.Add("@p_organizationId", CommonFunction.OrganizationId);
            parameters.Add("@p_categoryId", categoryId);
            var data = await DbContext.Connection.QueryAsync<object>(storeName, parameters, transaction:DbContext.Transaction);
            return data;
        }

        public async Task<object> GetSaleReportByItems(string inventoryItemId, string customerId, DateTime? startDate, DateTime? endDate, string categoryId)
        {
            var storeName = "Proc_SaleReportByItems";
            var parameters = new DynamicParameters();
            parameters.Add("@p_startDate", startDate);
            parameters.Add("@p_endDate", endDate);
            parameters.Add("@p_organizationId", CommonFunction.OrganizationId);
            parameters.Add("@p_categoryId", categoryId);
            parameters.Add("@p_inventoryItemId", inventoryItemId);
            parameters.Add("@p_customerId", customerId);
            var data = await DbContext.Connection.QueryAsync<object>(storeName, parameters, transaction: DbContext.Transaction);
            return data;
        }

        public async Task<object> GetRevenueByCustomer(string customerId, DateTime? startDate, DateTime? endDate)
        {
            var storeName = "Proc_Report_RevenueByCustomer";
            var parameters = new DynamicParameters();
            parameters.Add("@p_startDate", startDate);
            parameters.Add("@p_endDate", endDate);
            parameters.Add("@p_organizationId", CommonFunction.OrganizationId);
            parameters.Add("@p_customerId", customerId);
            var data = await DbContext.Connection.QueryAsync<object>(storeName, parameters, transaction: DbContext.Transaction);
            return data;
        }

        public async Task<object> GetGetOrdersByCustomer(string? customerId, DateTime? startDate, DateTime? endDate)
        {
            var storeName = "Proc_Customer_GetOrders";
            var parameters = new DynamicParameters();
            parameters.Add("@p_startDate", startDate);
            parameters.Add("@p_endDate", endDate);
            parameters.Add("@p_organizationId", CommonFunction.OrganizationId);
            parameters.Add("@p_customerId", customerId);
            var data = await DbContext.Connection.QueryAsync<object>(storeName, parameters, transaction: DbContext.Transaction);
            return data;
        }
    }
}
