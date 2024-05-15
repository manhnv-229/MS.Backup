using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.Core.Entities;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Repositories;
using MS.Core.Interfaces.Services;
using Ms.Api.Controllers;

namespace Ms.Api.Controllers
{
    public class InventoryItemCategoriesController : BaseController<InventoryItemCategory>
    {
        public InventoryItemCategoriesController(IInventoryItemCategoryRepository repository, IInventoryItemCategoryService baseService) : base(repository, baseService)
        {
        }
    }
}
