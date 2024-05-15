using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.ApplicationCore.Authorization;
using MS.ApplicationCore.Entities;
using MS.ApplicationCore.Interfaces;
using MS.ApplicationCore.Interfaces.Repositories;
using MS.ApplicationCore.Interfaces.Services;
using MS.ApplicationCore.MSEnums;
using MS.ThaiSanlonV2.Api.Controllers;

namespace MS.ThaiSalonV2.Api.Controllers
{
    [Authorize(MSRole.Member)]
    public class InvoicesController : BaseController<Invoice>
    {
        public InvoicesController(IInvoiceRepository repository, IInvoiceService service) : base(repository, service)
        {
        }
        [Authorize(MSRole.Member)]
        public async override Task<IActionResult> Get()
        {
            return await base.Get();
        }

        [Authorize(MSRole.Member)]
        public async override Task<IActionResult> Get(string id)
        {
            return await base.Get(id);
        }

        [Authorize(MSRole.Member)]
        public async override Task<IActionResult> Get(string key, int limit, int offset)
        {
            return await base.Get(key, limit, offset);
        }

        [Authorize(MSRole.Member)]
        public async override Task<IActionResult> Post([FromBody] Invoice entity)
        {
            return await base.Post(entity);
        }
        [Authorize(MSRole.Member)]
        public async override Task<IActionResult> Put(string id, [FromBody] Invoice entity)
        {
            return await base.Put(id, entity);
        }
    }
}
