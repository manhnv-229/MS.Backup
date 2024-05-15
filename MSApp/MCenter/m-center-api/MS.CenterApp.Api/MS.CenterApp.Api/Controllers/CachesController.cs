using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.Core.Interfaces;
using System.Security.Policy;

namespace MS.CenterApp.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CachesController : ControllerBase
    {
        IUnitOfWork _uow;
        IHttpClientFactory _httpClientFactory;
        IServiceProvider _serviceProvider;
        public CachesController(IUnitOfWork uow, IServiceProvider serviceProvider)
        {
            _uow = uow;
            _serviceProvider = serviceProvider;
            _httpClientFactory = serviceProvider.GetService(typeof(IHttpClientFactory)) as IHttpClientFactory;
        }

        [HttpPost("app-db-caches/{appCode}/reset")]
        public async Task<IActionResult> ResetCacheByAppCode([FromRoute]string appCode)
        {
            var apps = await _uow.MSApps.GetAllAsync();
            var app = apps.FirstOrDefault(app => app.MSAppCode == appCode);
            if (app != null)
            {
                var apiUrl = $"{app.ApiUrl}/caches/db-server-cache";
                var httpClient = _httpClientFactory.CreateClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, apiUrl);
                var res = await httpClient.SendAsync(request);
                var resContent = await res.Content.ReadAsStringAsync();
                if (res.IsSuccessStatusCode)
                {
                return Ok(app);

                }
                else
                {
                    return BadRequest(resContent);
                }
            }
            else
            {
                return NotFound();
            }

        }
    }
}
