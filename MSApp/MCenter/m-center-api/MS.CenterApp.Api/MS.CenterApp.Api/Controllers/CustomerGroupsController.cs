using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.Core.Entities;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Repositories;
using MS.Core.Interfaces.Services;
using MS.CenterApp.Api.Controllers;

namespace MS.CenterApp.Api.Controllers
{
    public class CustomerGroupsController : BaseController<CustomerGroup>
    {
        public CustomerGroupsController(ICustomerGroupRepository repository, ICustomerGroupService baseService) : base(repository, baseService)
        {
        }
    }
}
