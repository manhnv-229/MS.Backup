using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.ApplicationCore.Authorization;
using MS.ApplicationCore.Entities.Auth;
using MS.ApplicationCore.Interfaces;
using MS.ThaiSanlonV2.Api.Controllers;

namespace MS.ThaiSalonV2.Api.Controllers
{
    [Authorize(ApplicationCore.MSEnums.MSRole.Member)]
    public class RolesController : BaseController<Role>
    {
        public RolesController(IRepository<Role> repository, IBaseService<Role> baseService) : base(repository, baseService)
        {
        }
    }
}
