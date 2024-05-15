using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.Core.Entities;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Repositories;
using MS.Core.Interfaces.Services;
using Ms.eShop.Api.Controllers;

namespace MS.ThaiSalonV2.Api.Controllers
{
    public class UnitsController : BaseController<Unit>
    {
        public UnitsController(IUnitRepository repository, IUnitService baseService) : base(repository, baseService)
        {
        }
    }
}
