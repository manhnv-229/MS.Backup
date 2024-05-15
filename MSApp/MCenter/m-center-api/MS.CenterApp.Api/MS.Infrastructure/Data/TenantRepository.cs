using Dapper;
using MS.Core.Entities;
using MS.Core.Interfaces.Repositories;
using MS.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Infrastructure.Data
{
    public class TenantRepository : DapperRepository<Tenant>, ITenantRepository
    {
        public TenantRepository(IMSDatabaseContext dbContext) : base(dbContext)
        {
        }

        public async  Task<IEnumerable<object>> GetAllTenants()
        {
            var sql = "SELECT *  FROM Views_Tenant";
            var data = await DbContext.Connection.QueryAsync<object>(sql);
            return data;
        }
    }
}
