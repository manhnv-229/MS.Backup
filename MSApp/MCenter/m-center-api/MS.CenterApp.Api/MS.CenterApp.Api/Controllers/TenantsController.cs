using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.Core.DTOs;
using MS.Core.Entities;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Repositories;
using MS.Core.Interfaces.Services;
using MS.Core.Utilities;

namespace MS.CenterApp.Api.Controllers
{
    public class TenantsController : BaseController<Tenant>
    {
        ITenantRepository _repository;
        ITenantService _service;
        IConfiguration _configuration;
        ICloudflareApi _cloudflareApi;
        public TenantsController(ITenantService service, ITenantRepository repository, IConfiguration configuration, ICloudflareApi cloudflareApi) : base(repository, service)
        {
            _service = service;
            _repository = repository;
            _configuration = configuration;
            _cloudflareApi = cloudflareApi;
        }

        public async override Task<IActionResult> Get()
        {
            var data = await _repository.GetAllTenants();
            return Ok(data);
        }

        [HttpPost("admin-register")]
        public async Task<IActionResult> RegisterTenant(TenantRegister tenant)
        {
            await _service.RegisterAsync(tenant);
            return StatusCode(201, tenant);
        }


        [HttpGet("domain-default/{appcode}")]
        public async Task<IActionResult> GetRegisterDefault(string appcode)
        {
            var domain= _configuration[$"WebServer:WebDomain:{appcode}"];
            return Ok(domain);
        }


        [HttpGet("cloudflare/zones")]
        public async Task<IActionResult> GetZones()
        {
            var zones = await _cloudflareApi.GetZones();
            return Ok(zones);
        }
    }
}
