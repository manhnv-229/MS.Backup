using MS.ApplicationCore.DTOs;
using MS.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.ApplicationCore.Interfaces
{
    public interface IEmployeeRepository:IRepository<Employee>
    {
        Task<string> GetNewCode();
        new Task<EmployeeInfo> GetByIdAsync(object pksFields);

        Task<IEnumerable<StatisticInvoiceByEmployee>> GetStatisticInvoiceByEmployees(DateTime startDate,DateTime endDate);
    }
}
