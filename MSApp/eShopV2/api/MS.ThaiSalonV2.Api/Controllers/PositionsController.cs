using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.ApplicationCore.Entities;
using MS.ApplicationCore.Interfaces;
using MS.ThaiSanlonV2.Api.Controllers;

namespace MS.ThaiSalonV2.Api.Controllers
{
    public class PositionsController : BaseController<EmployeePosition>
    {
        public PositionsController(IPositionRepository repository, IPositionService baseService) : base(repository, baseService)
        {
        }
    }
}
