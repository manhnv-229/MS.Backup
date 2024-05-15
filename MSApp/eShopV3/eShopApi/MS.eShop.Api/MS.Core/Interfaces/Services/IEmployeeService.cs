using Microsoft.AspNetCore.Http;
using MS.Core.DTOs;
using MS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Interfaces.Services
{
    public interface IEmployeeService:IBaseService<Employee>
    {
        Task<int> AddAsync(EmployeeInfo employee, IFormFile file);
        Task<int> UpdateAsync(EmployeeInfo employee, IFormFile file);
    }
}
