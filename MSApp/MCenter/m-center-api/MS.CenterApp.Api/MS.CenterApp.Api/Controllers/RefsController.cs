using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.Core.Authorization;
using MS.Core.Entities;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Repositories;
using MS.Core.Interfaces.Services;
using MS.Core.MSEnums;
using MS.CenterApp.Api.Controllers;
using System.Net.WebSockets;
using Mysqlx.Crud;

namespace MS.CenterApp.Api.Controllers
{
    [Authorize(MSRole.Member)]
    public class RefsController : BaseController<Ref>
    {
        IUnitOfWork _unitOfWork;
        public RefsController(IRefRepository repository, IRefService service, IUnitOfWork unitOfWork) : base(repository, service)
        {
            _unitOfWork = unitOfWork;
        }
        [Authorize(MSRole.Member)]
        public async override Task<IActionResult> Get()
        {
            var httpRequest = HttpContext.Request;
            int type  = 0;
            int limit = 0;
            int offset = 0;
            var key = httpRequest?.Query["key"];
            int.TryParse(httpRequest?.Query["refType"], out type);
            int.TryParse(httpRequest?.Query["limit"], out limit);
            int.TryParse(httpRequest?.Query["offset"], out offset);

            var refType = (RefType)type;

            var res = await _unitOfWork.Refs.GetRefsByRefTypePaging(refType, limit, offset, key);
            return Ok(res);
        }

        [Authorize(MSRole.Member)]
        public async override Task<IActionResult> Get(string id)
        {
            var data = await _unitOfWork.Refs.GetRefById(Guid.Parse(id));
            return Ok(data);
        }

        [Authorize(MSRole.Member)]
        public async override Task<IActionResult> Get(string key, int limit, int offset)
        {
            return await base.Get(key, limit, offset);
        }

        [Authorize(MSRole.Member)]
        public async override Task<IActionResult> Post([FromBody] Ref entity, [FromForm] IFormCollection? form = null)
        {
            return await base.Post(entity);
        }
        [Authorize(MSRole.Member)]
        public async override Task<IActionResult> Put(string id, [FromBody] Ref entity, [FromForm] IFormCollection? formData = null)
        {
            return await base.Put(id, entity);
        }

        #region Phiếu nhập kho
        [Authorize(MSRole.Member)]
        [HttpPost("stock-inventory")]
        public async Task<IActionResult> InsertStockInventory([FromBody] Ref entity, [FromForm] IFormCollection? form = null)
        {
            return await base.Post(entity);
        }

        [Authorize(MSRole.Member)]
        [HttpPut("stock-inventory")]
        public async Task<IActionResult> UpdateStockInventory(string id, [FromBody] Ref entity, [FromForm] IFormCollection? formData = null)
        {
            return await base.Put(id, entity);
        }
        #endregion

        #region Phiếu xuất kho

        #endregion
    }
}
