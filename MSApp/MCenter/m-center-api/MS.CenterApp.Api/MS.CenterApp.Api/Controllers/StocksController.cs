using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.Core.Entities;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Repositories;
using MS.Core.Interfaces.Services;
using MS.CenterApp.Api.Controllers;

namespace MS.CenterApp.Api.Controllers
{
    public class StocksController : BaseController<Stock>
    {
        public StocksController(IRepository<Stock> repository, IBaseService<Stock> baseService) : base(repository, baseService)
        {
        }
    }
}
