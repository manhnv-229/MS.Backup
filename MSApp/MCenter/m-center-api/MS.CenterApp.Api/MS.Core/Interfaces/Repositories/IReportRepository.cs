using MS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Interfaces.Repositories
{
    public interface IReportRepository:IRepository<Report>
    {
        Task<object> GetProfitAmountByItems(DateTime? startDate, DateTime? endDate, string categoryId);
        Task<object> GetSaleReportByItems(string? inventoryItemId, string customerId, DateTime? startDate, DateTime? endDate, string? categoryId);
        Task<object> GetRevenueByCustomer(string? customerId, DateTime? startDate, DateTime? endDate);
        Task<object> GetGetOrdersByCustomer(string? customerId, DateTime? startDate, DateTime? endDate);
    }
}
