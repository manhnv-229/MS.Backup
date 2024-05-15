using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.Core.Authorization;
using MS.Core.Entities;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Repositories;
using MS.Core.Interfaces.Services;
using MS.Core.MSEnums;
using Ms.Api.Controllers;

namespace Ms.Api.Controllers
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
        public async override Task<IActionResult> Post([FromBody] Invoice entity, IFormCollection? form = null)
        {
            return await base.Post(entity);
        }
        [Authorize(MSRole.Member)]
        public async override Task<IActionResult> Put(string id, [FromBody] Invoice entity, IFormCollection? formData = null)
        {
            return await base.Put(id, entity);
        }
    }
}
