using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.ApplicationCore.Entities;
using MS.ApplicationCore.Interfaces;
using MS.ApplicationCore.Interfaces.Repositories;
using MS.ApplicationCore.Interfaces.Services;
using MS.ThaiSanlonV2.Api.Controllers;

namespace MS.ThaiSalonV2.Api.Controllers
{
    public class UnitsController : BaseController<Unit>
    {
        public UnitsController(IUnitRepository repository, IUnitService baseService) : base(repository, baseService)
        {
        }
    }
}
