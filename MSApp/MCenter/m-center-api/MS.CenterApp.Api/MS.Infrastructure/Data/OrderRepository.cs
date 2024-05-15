using Dapper;
using MS.Core.DTOs;
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
    public class OrderRepository : DapperRepository<Ref>, IOrderRepository
    {
        public OrderRepository(IMSDatabaseContext dbContext) : base(dbContext)
        {
        }

        public async Task<InvoiceSlotDto> GetOrderDetailById(Guid refId)
        {
            var sql = "SELECT * FROM Ref r WHERE r.RefId = @refId";
            var parameters = new DynamicParameters();
            parameters.Add("@refId", refId);
            var data = await DbContext.Connection.QueryFirstOrDefaultAsync<InvoiceSlotDto>(sql, parameters, transaction:DbContext.Transaction);
            if (data != null)
            {
                var sqlRefDetail = "SELECT * FROM View_RefDetail r WHERE r.RefId = @refId";
                var sqlSlotInvoice = "SELECT * FROM View_SlotInvoice r WHERE r.RefId = @refId";
                var sqlServiceInvoice = "SELECT * FROM View_ServiceInvoice r WHERE r.RefId = @refId";
                var sqlOrgInfo = "SELECT * FROM Organization o WHERE o.OrganizationId = @orgId";
                parameters.Add("@orgId", CommonFunction.OrganizationId);
                data.RefDetail = (await DbContext.Connection.QueryAsync<RefDetailResponse>(sqlRefDetail, parameters, transaction:DbContext.Transaction))?.ToList();
                data.SlotInvoices = (await DbContext.Connection.QueryAsync<SlotInvoiceDto>(sqlSlotInvoice, parameters, transaction: DbContext.Transaction))?.ToList();
                data.Slots = (await DbContext.Connection.QueryAsync<SlotDto>(sqlSlotInvoice, parameters, transaction: DbContext.Transaction))?.ToList();
                data.ServiceInvoices = (await DbContext.Connection.QueryAsync<ServiceInvoiceDto>(sqlServiceInvoice, parameters, transaction: DbContext.Transaction))?.ToList();
                data.Organization = await DbContext.Connection.QueryFirstOrDefaultAsync<Organization>(sqlOrgInfo, parameters, transaction: DbContext.Transaction);            }
            return data;
        }
    }
}
