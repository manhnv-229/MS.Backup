using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.Core.Entities.Auth;
using Ms.eShop.Api.Controllers;
using MS.Core.Interfaces;
using MS.Core.Entities;

namespace MS.eShop.Api.Controllers
{
    public class DistributorsController : BaseController<Distributor>
    {
        public DistributorsController(IRepository<Distributor> repository, IBaseService<Distributor> baseService) : base(repository, baseService)
        {
        }
    }
}
