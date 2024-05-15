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
    public class CustomersController : BaseController<Customer>
    {
        public CustomersController(ICustomerRepository repository, ICustomerService baseService) : base(repository, baseService)
        {
        }

        [Authorize(MSRole.Member)]
        public async override Task<IActionResult> Post([FromBody] Customer entity, [FromForm] IFormCollection? form = null)
        {
            return await base.Post(entity, form);
        }

        [Authorize(MSRole.Member)]
        public async override Task<IActionResult> Put(string id, [FromBody] Customer entity, [FromForm] IFormCollection? formData = null)
        {
            return await base.Put(id, entity);
        }
    }
}
