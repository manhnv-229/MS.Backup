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
    public class CustomersController : BaseController<Customer>
    {
        public CustomersController(ICustomerRepository repository, ICustomerService baseService) : base(repository, baseService)
        {
        }

        [Authorize(MSRole.Member)]
        public async override Task<IActionResult> Post([FromBody] Customer entity)
        {
            return await base.Post(entity);
        }

        [Authorize(MSRole.Member)]
        public async override Task<IActionResult> Put(string id, [FromBody] Customer entity)
        {
            return await base.Put(id, entity);
        }
    }
}
