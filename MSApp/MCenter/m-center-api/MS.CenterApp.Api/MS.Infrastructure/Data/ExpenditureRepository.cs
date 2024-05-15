using Dapper;
using MISA.FM.Infrastructure.Repositories;
using MS.Core.DTOs;
using MS.Core.Entities;
using MS.Core.Interfaces;
using MS.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MS.Infrastructure.Interfaces;

namespace MS.Infrastructure.Data
{
    public class ExpenditureRepository : DapperRepository<Expenditure>,IExpenditureRepository
    {
        public ExpenditureRepository(IMSDatabaseContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Expenditure>> GetRevenues()
        {
            var storeName = "Proc_Expenditure_GetRevenues";
            var parameters = new DynamicParameters();
            var data = await DbContext.Connection.QueryAsync<Expenditure>(storeName, param: parameters, transaction: DbContext.Transaction, commandType: System.Data.CommandType.StoredProcedure);
            return data;
        }

        public async Task<IEnumerable<Expenditure>> GetExpenditures()
        {
            var storeName = "Proc_Expenditure_GetExpenditures";
            var parameters = new DynamicParameters();
            var data = await DbContext.Connection.QueryAsync<Expenditure>(storeName, param: parameters, transaction: DbContext.Transaction, commandType: System.Data.CommandType.StoredProcedure);
            return data;
        }

        public async Task<FundInfo> GetGeneralInfo()
        {
            var storeName = "Proc_Expenditure_GetGeneralInfo";
            var parameters = new DynamicParameters();
            var data = await DbContext.Connection.QueryFirstOrDefaultAsync<FundInfo>(storeName, param: parameters, transaction: DbContext.Transaction, commandType: System.Data.CommandType.StoredProcedure);
            return data;
        }
    }
}
