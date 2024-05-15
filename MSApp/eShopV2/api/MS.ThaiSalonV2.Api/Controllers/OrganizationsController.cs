using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.ApplicationCore.Authorization;
using MS.ApplicationCore.DTOs;
using MS.ApplicationCore.Entities;
using MS.ApplicationCore.Interfaces;
using MS.ApplicationCore.Interfaces.Repositories;
using MS.ApplicationCore.Interfaces.Services;
using MS.ApplicationCore.MSEnums;
using MS.Infrastructure.UnitOfWork;
using MS.ThaiSanlonV2.Api.Controllers;
using Newtonsoft.Json;

namespace MS.ThaiSalonV2.Api.Controllers
{
    public class OrganizationsController : BaseController<Organization>
    {
        IUnitOfWork _uow;
        IOrganizationService _service;
        public OrganizationsController(IOrganizationRepository repository, IOrganizationService baseService, IUnitOfWork uow, IOrganizationService service) : base(repository, baseService)
        {
            _uow = uow;
            _service = service;
        }

        [Authorize(MSRole.Administrator)]
        public async override Task<IActionResult> Get(string key, int limit, int offset)
        {
            return await base.Get(key, limit, offset);
        }

        [Authorize(MSRole.SuperManager)]
        public async override Task<IActionResult> Put(string id, [FromBody] Organization entity)
        {
            return await base.Put(id, entity);
        }

        //[AllowAnonymous]
        //public async override Task<IActionResult> Patch(string id, [FromBody] object entity)
        //{
        //    return await base.Patch(id, entity);
        //}

        [AllowAnonymous]
        public async override Task<IActionResult> GetNewCode()
        {
            return await base.GetNewCode();
        }

        [Authorize(MSRole.Administrator)]
        [HttpPost("{id}/licenses")]
        public async Task<IActionResult> RenewLicense(MSLicenseRenew licenseInfo, Guid organizationId)
        {
            var res = await _service.RenewLicense(licenseInfo);
            return Ok(res);
        }

        [Authorize(MSRole.Administrator)]
        [HttpDelete("{id}/licenses")]
        public async Task<IActionResult> DeleteLicense(Guid id)
        {
            var res = await _service.DeleteLicense(id);
            return Ok(res);
        }

        [Authorize(MSRole.Administrator)]
        [HttpPost("backup")]
        public async Task<IActionResult> BackUpDatabase(Guid id)
        {
            await _service.BackupDatabase();
            return Ok();
        }

        [Authorize(MSRole.Administrator)]
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistic([FromQuery] DateTime date, bool sentEmail)
        {
            var res = await _service.GetOrgStatisticByDay(date, sentEmail);
            return Ok(res);
        }
    }
}
