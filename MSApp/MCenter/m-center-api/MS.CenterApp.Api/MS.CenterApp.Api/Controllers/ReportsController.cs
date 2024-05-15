using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.CenterApp.Api.Controllers;
using MS.Core.Authorization;
using MS.Core.Entities;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Repositories;
using MS.Core.Interfaces.Services;
using MS.Core.MSEnums;

namespace MS.eShop.Api.Controllers
{
    public class ReportsController : BaseController<Report>
    {
        IReportRepository _repository;
        IReportService _service;
        public ReportsController(IReportRepository repository, IReportService service) : base(repository, service)
        {
            _repository = repository;
            _service = service;
        }

        [Authorize(MSRole.SuperManager)]
        [HttpGet("profit")]
        public async Task<IActionResult> GetProfitAmountByItems([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, string categoryId)
        {
            var data = await _repository.GetProfitAmountByItems(startDate, endDate, categoryId);
            return Ok(data);
        }

        [Authorize(MSRole.SuperManager)]
        [HttpGet("sale")]
        public async Task<IActionResult> GetSaleReportByItems([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, string categoryId, string customerId, string inventoryItemId)
        {
            var data = await _repository.GetSaleReportByItems(inventoryItemId, customerId, startDate, endDate, categoryId);
            return Ok(data);
        }

        [Authorize(MSRole.SuperManager)]
        [HttpGet("customers/revenue")]
        public async Task<IActionResult> GetRevenueByCustomer([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, string customerId)
        {
            var data = await _repository.GetRevenueByCustomer(customerId, startDate, endDate);
            return Ok(data);
        }

        [Authorize(MSRole.SuperManager)]
        [HttpGet("customers/orders")]
        public async Task<IActionResult> GetGetOrdersByCustomer([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, string? customerId)
        {
            var data = await _repository.GetGetOrdersByCustomer(customerId, startDate, endDate);
            return Ok(data);
        }
    }
}
