using Dapper;
using MISA.FM.Infrastructure.Repositories;
using MS.Core.Entities.Auth;
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
    public class RoleRepository : DapperRepository<Role>, IRoleRepository
    {
        public RoleRepository(IMSDatabaseContext dbContext) : base(dbContext)
        {
        }

        public async Task DeleteAllRoleByUserId(object userId)
        {
            var sql = "DELETE FROM UserRole WHERE UserId = @UserId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            await DbContext.Connection.ExecuteAsync(sql, parameters, transaction: DbContext.Transaction);
        }

        public async Task<Role> GetRoleByUserId(object userId)
        {
            var sql = "SELECT r.* FROM UserRole u LEFT JOIN Role r ON u.RoleId = r.RoleId WHERE u.UserId = @UserId";
            DynamicParameters parameters= new DynamicParameters();
            parameters.Add("@UserId", userId);
            var role = await DbContext.Connection.QueryFirstOrDefaultAsync<Role>(sql, parameters,transaction:DbContext.Transaction);
            return role;
        }
    }
}
