using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.Core.Entities;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Services;
using Ms.eShop.Api.Controllers;
using System.Net.WebSockets;

namespace MS.ThaiSalonV2.Api.Controllers
{
    public class ProductsController : BaseController<Product>
    {
        IUnitOfWork _unitOfWork;
        public ProductsController(IProductRepository repository, IProductService baseService, IUnitOfWork unitOfWork) : base(repository, baseService)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("list-order")]
        public async Task<IActionResult> GetProductSaleResponses()
        {
            var products = await _unitOfWork.Products.GetProductSaleResponses();
            return Ok(products);
        }
    }
}
