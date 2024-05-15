using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.Core.Entities;
using Ms.Api.Controllers;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Services;
using MS.Core.Interfaces.Repositories;
using MS.Core.MSEnums;
using MS.Core.Authorization;

namespace MS.eShop.Api.Controllers
{
    [Route("/api/v1/stock-inventories")]
    public class StockInventorysController : BaseController<StockInventory>
    {
        IStockInventoryService _stockInventoryService;
        IStockInventoryRepository _stockInventoryRepository;
        public StockInventorysController(IStockInventoryService stockInventoryService, IStockInventoryRepository stockInventoryRepository) : base(stockInventoryRepository, stockInventoryService)
        {
            _stockInventoryService = stockInventoryService;
            _stockInventoryRepository = stockInventoryRepository;
        }

    }
}
