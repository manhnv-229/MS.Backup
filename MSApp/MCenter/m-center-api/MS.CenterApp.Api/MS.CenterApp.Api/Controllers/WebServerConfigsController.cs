using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.Core.Interfaces;

namespace MS.CenterApp.Api.Controllers
{
    [Route("api/v1/webserver-configs")]
    [ApiController]
    public class WebServerConfigsController : ControllerBase
    {
        IWebServerConfig _webServerConfig;

        public WebServerConfigsController(IWebServerConfig webServerConfig)
        {
            _webServerConfig = webServerConfig;
        }

        [HttpPost("nginx/restart")]
        public IActionResult RestartNginxServer()
        {
            _webServerConfig.RestartNginxServer();
            return Ok();
        }
    }
}
