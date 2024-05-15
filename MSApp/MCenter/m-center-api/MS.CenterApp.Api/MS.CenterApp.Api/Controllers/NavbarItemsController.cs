using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.CenterApp.Api.Controllers;
using MS.Core.Entities;
using MS.Core.Interfaces;

namespace MS.Api.Controllers
{
    public class NavbarItemsController : BaseController<NavbarItem>
    {
        public NavbarItemsController(IRepository<NavbarItem> repository, IBaseService<NavbarItem> baseService) : base(repository, baseService)
        {
        }
    }
}
