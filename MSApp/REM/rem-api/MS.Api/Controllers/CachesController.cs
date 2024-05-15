using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MS.Core.Authorization;

namespace MS.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CachesController : ControllerBase
    {
        IMemoryCache _memoryCache;
        IConfiguration _configuration;
        public CachesController(IMemoryCache memoryCache, IConfiguration configuration)
        {
            _memoryCache = memoryCache;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpDelete("db-server-cache")]
        public IActionResult ResetDbCache()
        {
            _memoryCache.Remove(_configuration["MSCatalogDb_Cache"]);
            return Ok();
        }
    }
}
