using MS.Core.DTOs;
using MS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Interfaces
{
    public interface IEmployeeRepository:IRepository<Employee>
    {
        Task<string> GetNewCode();
        new Task<EmployeeInfo> GetByIdAsync(object pksFields);

        Task<IEnumerable<StatisticInvoiceByEmployee>> GetStatisticInvoiceByEmployees(DateTime startDate,DateTime endDate,string? categoryId);
    }
}
