using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ms.Api.Controllers;
using MS.Core.Entities;
using MS.Core.Interfaces.Repositories;
using MS.Core.Interfaces.Services;
using MS.Core.Interfaces;

namespace MS.Api.Controllers
{
    [Route("api/v1/slot-groups")]
    public class SlotGroupsController : BaseController<SlotGroup>
    {
        ISlotGroupService _service;
        ISlotGroupRepository _repository;
        IUnitOfWork _uow;
        public SlotGroupsController(ISlotGroupRepository repository, ISlotGroupService service, IUnitOfWork uow) : base(repository, service)
        {
            _service = service;
            _repository = repository;
            _uow = uow;
        }
    }
}
