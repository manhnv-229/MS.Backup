using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MS.Core.Entities;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Repositories;
using MS.Core.Interfaces.Services;
using MS.Infrastructure.Cache;

namespace MS.CenterApp.Api.Controllers
{
    [Route("api/v1/system-configs")]
    public class SystemConfigsController : BaseController<SystemConfig>
    {
        ISystemConfigRepository _repository;
        ISystemConfigService _service;
        IConfiguration _configuration;
        IWebServerConfig _webServerConfig;
        IMemoryCache _memoryCache;
        public SystemConfigsController(ISystemConfigRepository repository, ISystemConfigService service, IConfiguration configuration, IWebServerConfig webServerConfig, IMemoryCache memoryCache) : base(repository, service)
        {
            _repository = repository;
            _service = service;
            _configuration = configuration;
            _webServerConfig = webServerConfig;
            _memoryCache = memoryCache;
        }

        public async override Task<IActionResult> Get()
        {
            /// Lấy tất cả các thiết lập mặc định:
            var data = await _repository.GetAllAsync();
            var domain = _configuration[$"WebServer:WebDomain:baza"];
            return Ok(new
            {
                configs = data,
                domain = domain,
            });
        }
    }
}
