using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ms.Api.Controllers;
using MS.Core.Entities;
using MS.Core.Interfaces.Repositories;
using MS.Core.Interfaces.Services;

namespace MS.Api.Controllers
{
    [Route("api/v1/service-groups")]
    public class ServiceGroupsController : BaseController<ServiceGroup>
    {
        IServiceGroupService _service;
        IServiceGroupRepository _repository;
        public ServiceGroupsController(IServiceGroupService service, IServiceGroupRepository repository):base(repository,service)
        {
            _service = service;
            _repository = repository;
        }

    }
}
