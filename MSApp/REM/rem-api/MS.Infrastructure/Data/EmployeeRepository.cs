using Dapper;
using MISA.FM.Infrastructure.Repositories;
using MS.Core.DTOs;
using MS.Core.Entities;
using MS.Core.Interfaces;
using MS.Core.Utilities;
using MS.Infrastructure.Interfaces;
using MS.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Infrastructure.Data
{
    public class EmployeeRepository : DapperRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(IMSDatabaseContext dbContext,AutoMapper.IMapper mapper) : base(dbContext,mapper)
        {
        }

        public async Task<string> GetNewCode()
        {
            var sql = "Proc_Employee_GetNewCode";
            var newCode = await DbContext.AppConnection.QueryFirstOrDefaultAsync<string>(sql,commandType:System.Data.CommandType.StoredProcedure);
            return newCode;
        }

        public new async Task<EmployeeInfo> GetByIdAsync(object pksFields)
        {
            var user = new UserRequest();
            var sqlUser = "SELECT u.*,r.RoleId FROM User u LEFT JOIN UserRole r ON u.UserId = r.UserId WHERE u.EmployeeId = @EmployeeId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@EmployeeId", pksFields);
            // Lấy thông tin nhân viên theo mã nhân viên:
            var employee = await DbContext.GetByIdAsync<EmployeeInfo>(pksFields);

            var exitsUser = await DbContext.AppConnection.QueryFirstOrDefaultAsync<UserRequest>(sqlUser, parameters, transaction: DbContext.Transaction);
            if (exitsUser != null)
            {
                employee.RoleId = exitsUser.RoleId;
                employee.User = exitsUser;
            }
            
            return employee;
        }

        public async Task<IEnumerable<StatisticInvoiceByEmployee>> GetStatisticInvoiceByEmployees(DateTime startDate, DateTime endDate,string? categoryId)
        {
            var storeName = "Proc_Statistic_GetInvoiceByEmployee";
            var parameters = new DynamicParameters();
            parameters.Add("@p_StartDate", startDate);
            parameters.Add("@p_EndDate", endDate);
            parameters.Add("@p_categoryId", categoryId);
            parameters.Add("@p_OrganizationId", CommonFunction.OrganizationId);
            var data = await DbContext.AppConnection.QueryAsync<StatisticInvoiceByEmployee>(storeName, param: parameters, transaction: DbContext.Transaction, commandType: System.Data.CommandType.StoredProcedure);
            return data;
        }
    }
}
