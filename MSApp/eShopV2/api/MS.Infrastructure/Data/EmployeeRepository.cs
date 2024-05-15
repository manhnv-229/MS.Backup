using Dapper;
using MS.ApplicationCore.DTOs;
using MS.ApplicationCore.Entities;
using MS.ApplicationCore.Interfaces;
using MS.ApplicationCore.Utilities;
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
        public EmployeeRepository(MySqlDbContext sqlDbContext) : base(sqlDbContext)
        {
        }

        public async Task<string> GetNewCode()
        {
            var sql = "Proc_Employee_GetNewCode";
            var newCode = await DbContext.Connection.QueryFirstOrDefaultAsync<string>(sql,commandType:System.Data.CommandType.StoredProcedure);
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

            var exitsUser = await DbContext.Connection.QueryFirstOrDefaultAsync<UserRequest>(sqlUser, parameters, transaction: DbContext.Transaction);
            if (exitsUser != null)
            {
                employee.RoleId = exitsUser.RoleId;
                employee.User = exitsUser;
            }
            
            return employee;
        }

        public async Task<IEnumerable<StatisticInvoiceByEmployee>> GetStatisticInvoiceByEmployees(DateTime startDate, DateTime endDate)
        {
            var storeName = "Proc_Statistic_GetInvoiceByEmployee";
            var parameters = new DynamicParameters();
            parameters.Add("@p_StartDate", startDate);
            parameters.Add("@p_EndDate", endDate);
            parameters.Add("@p_OrganizationId", CommonFunction.OrganizationId);
            var data = await DbContext.Connection.QueryAsync<StatisticInvoiceByEmployee>(storeName, param: parameters, transaction: DbContext.Transaction, commandType: System.Data.CommandType.StoredProcedure);
            return data;
        }
    }
}
