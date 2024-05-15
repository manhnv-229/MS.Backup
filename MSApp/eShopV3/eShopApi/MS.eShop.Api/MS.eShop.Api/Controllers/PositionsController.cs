using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.Core.Entities;
using MS.Core.Interfaces;
using Ms.eShop.Api.Controllers;

namespace MS.ThaiSalonV2.Api.Controllers
{
    public class PositionsController : BaseController<EmployeePosition>
    {
        public PositionsController(IPositionRepository repository, IPositionService baseService) : base(repository, baseService)
        {
        }
    }
}
