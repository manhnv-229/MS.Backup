using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.Core.Authorization;
using MS.Core.DTOs;
using MS.Core.Entities;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Repositories;
using MS.Core.Interfaces.Services;
using MS.Core.Services;

namespace MS.Api.Controllers
{
    [Route("api/v1/orders")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        ISlotRepository _slotRepository;
        ISlotService _slotService;
        IUnitOfWork _unitOfWork;
        IOrderService _orderService;

        public OrdersController(IServiceProvider serviceProvider)
        {
            _slotRepository = serviceProvider.GetService(typeof(ISlotRepository)) as ISlotRepository;
            _slotService = serviceProvider.GetService(typeof(ISlotService)) as ISlotService; ;
            _unitOfWork = serviceProvider.GetService(typeof(IUnitOfWork)) as IUnitOfWork; ;
            _orderService = serviceProvider.GetService(typeof(IOrderService)) as IOrderService; ;
        }

        [HttpGet("slots")]
        public async Task<IActionResult> GetAllSlots()
        {
            var data = await _unitOfWork.Slots.GetAllAsync();
            return Ok(data);
        }


        [HttpGet("{refId}")]
        public async Task<IActionResult> GetAllSlots(Guid refId)
        {
            var data = await _unitOfWork.Orders.GetOrderDetailById(refId);
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] InvoiceSlotDto slot)
        {
            await _orderService.CreateSlotOrderAsync(slot);
            return Ok();
        }


        [HttpPut("{refId}")]
        public async Task<IActionResult> UpdateSlotOrder(Guid refId, [FromBody] InvoiceSlotDto obj)
        {
            await _orderService.UpdateSlotOrder(obj);
            return Ok();
        }
    }
}
