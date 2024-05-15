using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.Core.Entities;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Services;
using Ms.Api.Controllers;
using System.Net.WebSockets;
using MS.Core.Entities.Auth;
using Newtonsoft.Json;
using MS.Core.MSEnums;
using MS.Core.Authorization;

namespace Ms.Api.Controllers
{
    [Route("/api/v1/inventories")]
    public class InventoryItemsController : BaseController<InventoryItem>
    {
        IUnitOfWork _unitOfWork;
        IInventoryItemService _service;
        public InventoryItemsController(IInventoryItemRepository repository, IInventoryItemService baseService, IUnitOfWork unitOfWork) : base(repository, baseService)
        {
            _unitOfWork = unitOfWork;
            _service = baseService;
        }

        [Authorize(MSRole.Member)]
        [HttpGet()]
        public async override Task<IActionResult> Get()
        {
            var inventoryItems = await _unitOfWork.InventoryItems.GetInventoryItemAdminResponses();
            return Ok(inventoryItems);
        }

        [Authorize(MSRole.Member)]
        [HttpGet("paging")]
        public override Task<IActionResult> Get(string key, int limit, int offset)
        {
            return base.Get(key, limit, offset);
        }

        [HttpGet("list-order")]
        public async Task<IActionResult> GetInventoryItemSaleResponses()
        {
            var products = await _unitOfWork.InventoryItems.GetInventoryItemSaleResponses();
            return Ok(products);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> FilterInventoryItemSaleResponses([FromQuery]string key)
        {
            var products = await _unitOfWork.InventoryItems.FilterInventoryItemSaleResponses(key);
            return Ok(products);
        }

        [HttpPost("create")]
        public async Task<IActionResult> AddNew()
        {
            //TODO: ĐAng làm chỗ này 1
            var httpRequest = HttpContext.Request;
            var inventoryItem = new InventoryItem();

            if (httpRequest.Form["inventory"].FirstOrDefault() != null)
            {
                inventoryItem = JsonConvert.DeserializeObject<InventoryItem>(httpRequest.Form["inventory"].First());
            }
            var file = httpRequest?.Form?.Files?.FirstOrDefault();
            var res = await _service.AddNewService(inventoryItem, file);
            return Ok(res);
        }

        public async override Task<IActionResult> Post([FromBody] InventoryItem entity, IFormCollection? formData = null)
        {
            // need to check if it is actually a form content type, as formModel may be bound to an empty instance
            var inventoryItem = new InventoryItem();
            if (Request.HasFormContentType && formData != null)
            {
                inventoryItem = JsonConvert.DeserializeObject<InventoryItem>(formData["inventory"].First());
                var file = formData.Files?.FirstOrDefault();
                var res = await _service.AddNewService(inventoryItem, file);
                return Ok(res);
            }
            return await base.Post(entity, formData);
        }

        [HttpGet("newCodeAuto")]
        public async Task<IActionResult> GetNewCodeAuto([FromQuery] string name)
        {
            var newCode = await _service.GetNewCodeAuto(name);
            return Ok(newCode);
        }
    }
}
