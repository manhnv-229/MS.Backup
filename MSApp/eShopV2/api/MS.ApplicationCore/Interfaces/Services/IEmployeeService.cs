using Microsoft.AspNetCore.Http;
using MS.ApplicationCore.DTOs;
using MS.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.ApplicationCore.Interfaces.Services
{
    public interface IEmployeeService:IBaseService<Employee>
    {
        Task<int> AddAsync(EmployeeInfo employee, IFormFile file);
        Task<int> UpdateAsync(EmployeeInfo employee, IFormFile file);
    }
}
