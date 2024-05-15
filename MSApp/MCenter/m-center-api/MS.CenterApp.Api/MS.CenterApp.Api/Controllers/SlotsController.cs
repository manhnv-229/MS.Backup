using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MS.CenterApp.Api.Controllers;
using MS.Core.Entities;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Repositories;
using MS.Core.Interfaces.Services;
using MS.Infrastructure.UnitOfWork;
using static Dapper.SqlMapper;

namespace MS.Api.Controllers
{
    public class SlotsController : BaseController<Slot>
    {
        ISlotService _service;
        ISlotRepository _repository;
        IUnitOfWork _uow;
        public SlotsController(ISlotRepository repository, ISlotService service, IUnitOfWork uow) : base(repository, service)
        {
            _service = service;
            _repository = repository;
            _uow = uow;
        }

        public async override Task<IActionResult> Get()
        {
            var slots = await _uow.Slots.GetAllAsync();
            return Ok(slots);
        }
    }
}
