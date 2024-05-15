using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.ApplicationCore.Authorization;
using MS.ApplicationCore.DTOs;
using MS.ApplicationCore.Entities;
using MS.ApplicationCore.Interfaces;
using MS.ApplicationCore.Interfaces.Services;
using MS.ApplicationCore.MSEnums;
using MS.Infrastructure.UnitOfWork;
using MS.ThaiSanlonV2.Api.Controllers;
using Newtonsoft.Json;
using System.Net;

namespace MS.ThaiSalonV2.Api.Controllers
{
    public class EmployeesController : BaseController<Employee>
    {
        IEmployeeService _employeeService;
        IEmployeeRepository _employeeRepository;
        IUnitOfWork _unitOfWork;

        public EmployeesController(IEmployeeService employeeService, IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork) : base(employeeRepository, employeeService)
        {
            _employeeService = employeeService;
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
        }

        public override async Task<IActionResult> Get(string id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            return Ok(employee);
        }

        [HttpGet("new-code")]
        public override async Task<IActionResult> GetNewCode()
        {
            var data = await _unitOfWork.Employees.GetNewCode();
            return Ok(data);
        }

        [Authorize(MSRole.SuperManager)]
        [HttpPost("create")]
        public async Task<IActionResult> Post()
        {
            var httpRequest = HttpContext.Request;
            EmployeeInfo employee = new EmployeeInfo();
            if (httpRequest.Form["employee"].FirstOrDefault() != null)
            {
                employee = JsonConvert.DeserializeObject<EmployeeInfo>(httpRequest.Form["employee"].First());
            }
            var file = httpRequest.Form?.Files?.FirstOrDefault();
            var res = await _employeeService.AddAsync(employee, file);

            if (res > 0)
                return Ok();
            else
                return StatusCode((int)HttpStatusCode.InternalServerError, res);
        }

        [Authorize(MSRole.SuperManager)]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateEmployee()
        {
            var httpRequest = HttpContext.Request;
            EmployeeInfo employee = new EmployeeInfo();
            if (httpRequest.Form["employee"].FirstOrDefault() != null)
            {
                employee = JsonConvert.DeserializeObject<EmployeeInfo>(httpRequest.Form["employee"].First());
            }
            var file = httpRequest.Form?.Files?.FirstOrDefault();
            var res = await _employeeService.UpdateAsync(employee, file);

            if (res > 0)
                return Ok();
            else
                return StatusCode((int)HttpStatusCode.InternalServerError, res);
        }

        [Authorize(MSRole.SuperManager)]
        [HttpGet("statistics/invoice-timerange")]
        public async Task<IActionResult> GetStatisticInvoiceByEmployees(DateTime startDate, DateTime endDate)
        {
            //var startDate = (DateTime)timeInfo?.StartDateTime;
            //var endDate = (DateTime)timeInfo?.EndDateTime;
            var res = await _unitOfWork.Employees.GetStatisticInvoiceByEmployees(startDate, endDate);
            return Ok(res);
        }
    }
}
