using Microsoft.AspNetCore.Mvc;
using MS.ApplicationCore.Authorization;
using MS.ApplicationCore.MSEnums;
using MS.ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MS.ThaiSanlonV2.Api.Controllers
{
    
    [Route("/api/v1/[controller]")]
    [ApiController]
    public class BaseController<TEntity> : ControllerBase where TEntity: class
    {
        IRepository<TEntity> _repository;
        IBaseService<TEntity> _baseService;
        public BaseController(IRepository<TEntity> repository, IBaseService<TEntity> baseService)
        {
            _repository = repository;
            _baseService = baseService;
        }

        [Authorize(MSRole.Member)]
        [HttpGet()]
        public async virtual Task<IActionResult> Get()
        {
            var data = await _repository.GetAllAsync();
            return Ok(data);
        }

        // GET: api/<BaseController>
        [Authorize(MSRole.Member)]
        [HttpGet("paging")]
        public async virtual Task<IActionResult> Get(string key, int limit, int offset)
        {
            var data = await _baseService.GetFilterPaging(key, limit, offset);
            return Ok(data);
        }

        // GET api/<BaseController>/5
        [Authorize(MSRole.Member)]
        [HttpGet("{id}")]
        public async virtual Task<IActionResult> Get(string id)
        {
            var data = await _repository.GetByIdAsync(id);
            return Ok(data);
        }

        //[Authorize(MSRole.Member)]
        //[HttpGet("filter")]
        //public async virtual Task<IActionResult> GetPaging(string key, int limit, int offset)
        //{
        //    var data = await _baseService.GetFilterPaging(key,limit,offset);
        //    return Ok(data);
        //}

        // POST api/<BaseController>
        [Authorize(MSRole.SuperManager)]
        [HttpPost]
        public async virtual Task<IActionResult> Post([FromBody] TEntity entity)
        {
            var res = await _baseService.AddAsync(entity);
            if (res > 0)
                return Ok();
            else
                return StatusCode((int)HttpStatusCode.InternalServerError, res);
        }

        // PUT api/<BaseController>/5
        [Authorize(MSRole.Member)]
        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Put(string id, [FromBody] TEntity entity)
        {
            var res = await _baseService.UpdateAsync(entity,id);
            if (res > 0)
                return Ok();
            else
                return StatusCode((int)HttpStatusCode.InternalServerError, res);
        }

        [Authorize(MSRole.Member)]
        [HttpPatch("{id}")]
        public virtual async Task<IActionResult> Patch(string id, [FromBody] object entity)
        {
            var res = await _baseService.UpdateAsync(entity, id);
            if (res > 0)
                return Ok(res);
            else
                return StatusCode((int)HttpStatusCode.InternalServerError, res);
        }

        // DELETE api/<BaseController>/5
        [Authorize(MSRole.SuperManager)]
        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(string id)
        {
            var res = await _baseService.RemoveAsync(id);
            return Ok(res);
        }

        #region Other
        [Authorize(MSRole.Member)]
        [HttpGet("new-code")]
        public async virtual Task<IActionResult> GetNewCode()
        {
            var newCode = await _repository.GetNewCodeDynamic();
            return Ok(newCode);
        }
        #endregion
    }
}
