using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.Core.Entities;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Repositories;
using MS.Core.Interfaces.Services;
using Ms.Api.Controllers;

namespace Ms.Api.Controllers
{
    public class CustomerGroupsController : BaseController<CustomerGroup>
    {
        public CustomerGroupsController(ICustomerGroupRepository repository, ICustomerGroupService baseService) : base(repository, baseService)
        {
        }
    }
}
