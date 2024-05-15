using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.CenterApp.Api.Controllers;
using MS.Core.Entities;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Repositories;
using MS.Core.Interfaces.Services;

namespace MS.Api.Controllers
{
    public class ReservationsController : BaseController<Reservation>
    {
        IReservationService _service;
        IReservationRepository _repository;
        public ReservationsController(IReservationRepository repository, IReservationService baseService) : base(repository, baseService)
        {
            _service = baseService;
            _repository = repository;
        }
    }
}
