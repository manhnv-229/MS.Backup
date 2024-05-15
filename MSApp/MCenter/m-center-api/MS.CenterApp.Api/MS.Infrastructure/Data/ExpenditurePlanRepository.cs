using Dapper;
using MISA.FM.Infrastructure.Repositories;
using MS.Core.DTOs;
using MS.Core.Entities;
using MS.Core.Interfaces;
using MS.Core.MSEnums;
using MS.Infrastructure.Interfaces;
using MS.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Infrastructure.Data
{
    public class ExpenditurePlanRepository : DapperRepository<ExpenditurePlan>, IExpenditurePlanRepository
    {
        public ExpenditurePlanRepository(IMSDatabaseContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<ExpenditurePlanResponse>> GetExpenditurePlans(int? type=null)
        {
            var expenditurePlanType = "";
            // Nếu là thu
            if (type == null || type == 1)
            {
                expenditurePlanType+= $",{(int)ExpenditurePlanType.INCREMENT_ANNUAL},";
                expenditurePlanType += $",{(int)ExpenditurePlanType.INCREMENT_EVENT},";
                expenditurePlanType += $",{(int)ExpenditurePlanType.INCREMENT_OTHER},";
            }
            if(type == null || type == 2)
            {
                expenditurePlanType += $",{(int)ExpenditurePlanType.REDUCE_OTHER},";
                expenditurePlanType += $",{(int)ExpenditurePlanType.REDURE_EVENT},";
            }
            var sql = "Proc_ExpenditurePlan_GetAll";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@p_PlanTypes", expenditurePlanType);
            var expenditurePlans = await DbContext.Connection.QueryAsync<ExpenditurePlanResponse>(sql,param: parameters, transaction: DbContext.Transaction,commandType:System.Data.CommandType.StoredProcedure);
            return expenditurePlans;
        }

        public async Task<ExpenditurePlan> GetIncrementExpenditurePlanByEventId(object eventId)
        {
            var sql = "SELECT * FROM ExpenditurePlan e WHERE EventId = @EventId AND ExpenditurePlanType = @ExpenditureType";
            var parameters = new DynamicParameters();
            parameters.Add("@EventId", eventId);
            parameters.Add("@ExpenditureType", ExpenditurePlanType.INCREMENT_EVENT);
            var expenditurePlan = await DbContext.Connection.QueryFirstOrDefaultAsync<ExpenditurePlan>(sql, parameters, transaction:DbContext.Transaction);
            return expenditurePlan;
        }
    }
}
