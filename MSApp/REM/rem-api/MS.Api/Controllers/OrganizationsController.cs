using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.Core.Authorization;
using MS.Core.DTOs;
using MS.Core.Entities;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Repositories;
using MS.Core.Interfaces.Services;
using MS.Core.MSEnums;
using MS.Infrastructure.UnitOfWork;
using Ms.Api.Controllers;
using Newtonsoft.Json;

namespace Ms.Api.Controllers
{
    public class OrganizationsController : BaseController<Organization>
    {
        IUnitOfWork _uow;
        IOrganizationService _service;
        IOrganizationRepository _repository;

        public OrganizationsController(IOrganizationRepository repository, IOrganizationService baseService, IUnitOfWork uow, IOrganizationService service) : base(repository, baseService)
        {
            _uow = uow;
            _service = service;
            _repository = repository;
        }

        [Authorize(MSRole.Administrator)]
        public async override Task<IActionResult> Get(string key, int limit, int offset)
        {
            return await base.Get(key, limit, offset);
        }

        public async override Task<IActionResult> Get(string id)
        {
            var org = await _uow.Organizations.GetByIdAsync(id);
            return Ok(org);
        }

        [Authorize(MSRole.SuperManager)]
        public async override Task<IActionResult> Put(string id, [FromBody] Organization entity, [FromForm]IFormCollection? formData = null)
        {
            var res = await _service.UpdateAsync(entity, id);
            return Ok(res);
        }

        //[AllowAnonymous]
        //public async override Task<IActionResult> Patch(string id, [FromBody] object entity)
        //{
        //    return await base.Patch(id, entity);
        //}

        [AllowAnonymous]
        public async override Task<IActionResult> GetNewCode(string? str)
        {
            return await base.GetNewCode(str);
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
