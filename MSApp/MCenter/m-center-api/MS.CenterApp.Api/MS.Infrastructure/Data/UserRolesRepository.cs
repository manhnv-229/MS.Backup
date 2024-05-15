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
    public class UserRolesRepository : DapperRepository<UserRole>, IUserRolesRepository
    {
        public UserRolesRepository(IMSDatabaseContext dbContext) : base(dbContext)
        {
        }

        public async Task<Guid> GetUserRoleIdByRoleValue(int roleValue)
        {
            var sql = "SELECT RoleId FROM Role WHERE RoleValue = @RoleValue";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@RoleValue", roleValue);
            var userRoleId = await DbContext.Connection.QueryFirstOrDefaultAsync<Guid>(sql, param: parameters, transaction: DbContext.Transaction);
            return userRoleId;
        }
    }
}
