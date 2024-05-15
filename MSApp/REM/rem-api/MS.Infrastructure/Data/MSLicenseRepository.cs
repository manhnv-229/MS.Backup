using Dapper;
using MISA.FM.Infrastructure.Repositories;
using MS.Core.Entities;
using MS.Core.Entities.Auth;
using MS.Core.Interfaces.Repositories;
using MS.Infrastructure.Interfaces;
using MS.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Infrastructure.Data
{
    public class MSLicenseRepository : DapperRepository<MSLicense >, ILicenseRepository
    {
        public MSLicenseRepository(IMSDatabaseContext dbContext,AutoMapper.IMapper mapper) : base(dbContext,mapper)
        {
        }

        public async Task<MSLicense > GetLicenseByUserName(string userName)
        {
            var sql = "SELECT m.* FROM MSLicense m " +
                "INNER JOIN Organization o ON m.OrganizationId = o.OrganizationId " +
                "INNER JOIN Employee e ON e.OrganizationId = o.OrganizationId " +
                "INNER JOIN User u ON u.EmployeeId = e.EmployeeId LIMIT 1 " +
                "WHERE u.UserName = @UserName";
            var paremeters = new DynamicParameters();
            paremeters.Add("@UserName", userName);
            var license = await DbContext.AppConnection.QueryFirstOrDefaultAsync<MSLicense >(sql, paremeters);
            return license;
        }

        public async Task<MSLicense > GetLicenseByUserId(Guid userId)
        {
            var sql = "SELECT * FROM MSLicense m " +
                "INNER JOIN Organization o ON m.OrganizationId = o.OrganizationId " +
                "INNER JOIN Employee e ON o.OrganizationId = e.OrganizationId " +
                "INNER JOIN User u ON u.EmployeeId = e.EmployeeId " +
                "WHERE u.UserId = @UserId";
            var paremeters = new DynamicParameters();
            paremeters.Add("@UserId", userId);
            var license = await DbContext.AppConnection.QueryFirstOrDefaultAsync<MSLicense >(sql, paremeters);
            return license;
        }

        public async Task<MSLicense> GetLicenseByOrganizationId(Guid orgId)
        {
            var sql = "SELECT m.* FROM MSLicense m WHERE m.OrganizationId = @OrganizationId ORDER BY m.ExpiredDate DESC";
            var paremeters = new DynamicParameters();
            paremeters.Add("@OrganizationId", orgId);
            var license = await DbContext.AppConnection.QueryFirstOrDefaultAsync<MSLicense>(sql, paremeters, transaction:DbContext.Transaction);
            return license;
        }

        public async Task<int> DeleteLicenseByOrganizationId(Guid orgId)
        {
            var sql = "DELETE FROM MSLicense WHERE OrganizationId = @OrganizationId";
            var paremeters = new DynamicParameters();
            paremeters.Add("@OrganizationId", orgId);
            var res =  await DbContext.AppConnection.ExecuteAsync(sql,paremeters,transaction:DbContext.Transaction);
            return res;
        }
    }
}
