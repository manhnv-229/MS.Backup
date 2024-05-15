using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.Core.Authorization;
using MS.Core.Entities.Auth;
using MS.Core.Interfaces;
using Ms.Api.Controllers;

namespace Ms.Api.Controllers
{
    [Authorize(MS.Core.MSEnums.MSRole.Member)]
    public class RolesController : BaseController<Role>
    {
        public RolesController(IRepository<Role> repository, IBaseService<Role> baseService) : base(repository, baseService)
        {
        }
    }
}
