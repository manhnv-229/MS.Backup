using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.Core.Entities;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Repositories;
using MS.Core.Interfaces.Services;
using Ms.eShop.Api.Controllers;

namespace MS.ThaiSalonV2.Api.Controllers
{
    public class BranchsController : BaseController<Branch>
    {
        public BranchsController(IRepository<Branch> repository, IBaseService<Branch> baseService) : base(repository, baseService)
        {
        }
    }
}
