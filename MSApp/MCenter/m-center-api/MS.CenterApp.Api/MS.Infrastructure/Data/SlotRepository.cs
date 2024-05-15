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
    public class SlotRepository : DapperRepository<Slot>, ISlotRepository
    {
        public SlotRepository(IMSDatabaseContext dbContext) : base(dbContext)
        {
        }

        public new async Task<IEnumerable<object>> GetAllAsync()
        {
            var storeName = "Proc_Slot_GetAllSlotDto";
            var parameters = new DynamicParameters();
            parameters.Add("@p_orgId", CommonFunction.OrganizationId);
            var data = await DbContext.Connection.QueryAsync<SlotDto>(storeName,parameters, commandType:System.Data.CommandType.StoredProcedure);
            // Lấy tiếp thông tin Hoá đơn đang có của Slot nếu có:
            return data;
        }

        public Task<IEnumerable<object>> GetOtherDetailBySlotIdAsync(Guid slotId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<object>> GetServiceInvoiceBySlotIdAsync(Guid slotId)
        {
            throw new NotImplementedException();
        }

        public async Task<SlotDto?> GetSlotDtoAsync(Guid? slotId)
        {
            var sql = "SELECT * FROM Views_Slot s WHERE s.SlotId = @SlotId";
            var parameters = new DynamicParameters();
            parameters.Add("@SlotId", slotId);
            var slotDto = await DbContext.Connection.QueryFirstOrDefaultAsync<SlotDto>(sql,parameters,transaction:DbContext.Transaction);
            return slotDto;
        }
    }
}
