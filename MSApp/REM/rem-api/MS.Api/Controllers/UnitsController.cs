using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.Core.Entities;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Repositories;
using MS.Core.Interfaces.Services;
using Ms.Api.Controllers;
using System.Reflection;
using MS.Core.Helpers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Ms.Api.Controllers
{
    public class UnitsController : BaseController<Unit>
    {
        public UnitsController(IUnitRepository repository, IUnitService baseService) : base(repository, baseService)
        {
        }
    }
}
