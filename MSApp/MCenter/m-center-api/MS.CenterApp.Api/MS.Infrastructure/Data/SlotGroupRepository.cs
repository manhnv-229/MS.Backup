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
    public class SlotGroupRepository : DapperRepository<SlotGroup>, ISlotGroupRepository
    {
        public SlotGroupRepository(IMSDatabaseContext dbContext) : base(dbContext)
        {
        }

        public async override Task<IEnumerable<SlotGroup>> GetAllAsync()
        {
            var storeName = "Proc_SlotGroups_GetAll";
            var parameters = new DynamicParameters();
            parameters.Add("@p_OrganizationId",CommonFunction.OrganizationId);
            var data = await DbContext.Connection.QueryAsync<SlotGroup>(storeName, parameters, commandType: System.Data.CommandType.StoredProcedure);
            return data;
        }
    }
}
