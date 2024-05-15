using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.Core.Entities;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Repositories;
using MS.Core.Interfaces.Services;
using MS.CenterApp.Api.Controllers;

namespace MS.CenterApp.Api.Controllers
{
    public class InventoryItemCategoriesController : BaseController<InventoryItemCategory>
    {
        public InventoryItemCategoriesController(IInventoryItemCategoryRepository repository, IInventoryItemCategoryService baseService) : base(repository, baseService)
        {
        }
    }
}
