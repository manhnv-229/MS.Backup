using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.Core.Entities;
using MS.Core.Interfaces;
using MS.CenterApp.Api.Controllers;

namespace MS.CenterApp.Api.Controllers
{
    public class PositionsController : BaseController<EmployeePosition>
    {
        public PositionsController(IPositionRepository repository, IPositionService baseService) : base(repository, baseService)
        {
        }
    }
}
