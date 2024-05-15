using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.CenterApp.Api.Controllers;
using MS.Core.Entities;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Repositories;
using MS.Core.Interfaces.Services;

namespace MS.Api.Controllers
{
    [Route("/api/v1/reservation-details")]
    public class ReservationDetailsController : BaseController<ReservationDetail>
    {
        IReservationDetailService _service;
        IReservationDetailRepository _repository;
        public ReservationDetailsController(IReservationDetailRepository repository, IReservationDetailService baseService) : base(repository, baseService)
        {
            _service = baseService;
            _repository = repository;
        }
    }
}
