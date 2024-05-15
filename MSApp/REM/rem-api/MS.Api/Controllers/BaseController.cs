﻿using Microsoft.AspNetCore.Mvc;
using MS.Core.Authorization;
using MS.Core.MSEnums;
using MS.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MS.Core.Entities;
using Newtonsoft.Json;
using MS.Core.Helpers;
using Newtonsoft.Json.Linq;
using System.Reflection;
using MS.Core.Exceptions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Ms.Api.Controllers
{

    [Route("/api/v1/[controller]")]
    [ApiController]
    public class BaseController<TEntity> : ControllerBase where TEntity : class
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
        public async virtual Task<IActionResult> Post([FromBody] TEntity entity, [FromForm] IFormCollection? formData = null)
        {
            var res = await _baseService.AddAsync(entity);
            if (res > 0)
                return Ok();
            else
                return StatusCode((int)HttpStatusCode.InternalServerError, res);
        }

        [HttpPost("add-unit")]
        public async virtual Task<IActionResult> AddTest(JToken entity)
        {
            var entityType = typeof(TEntity);
            var modelAttribute = typeof(Unit).GetCustomAttribute(typeof(ModelClass<>), true);
            if (modelAttribute != null)
            {
                var method = modelAttribute.GetType().GetMethod("GetModelType");
                var modelType = method?.Invoke(null, null) as Type;
                if (modelType != null)
                {
                    entityType = modelType;

                }

            }
            //var model = Activator.CreateInstance(res);
            var jconvert = typeof(JsonConvert);
            var methodConvert = jconvert.GetMethods().FirstOrDefault(x =>
                                            x.Name.Equals("DeserializeObject", StringComparison.OrdinalIgnoreCase) &&
                                            x.IsGenericMethod && x.GetParameters().Length == 1)
                                            ?.MakeGenericMethod(entityType); ;
            var model = methodConvert?.Invoke(jconvert, new object[] { entity.ToString() });
            if (model != null)
            {
            var res = await _baseService.AddAsync(model);
            }
            else
            {
                throw new MSException("Lỗi thêm mới");
            }
            return Ok(model);
        }

        // PUT api/<BaseController>/5
        [Authorize(MSRole.Member)]
        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Put([FromRoute] string id, [FromBody] TEntity entity, [FromForm] IFormCollection? formData = null)
        {
            var res = await _baseService.UpdateAsync(entity, id, formData);
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
        public async virtual Task<IActionResult> GetNewCode([FromQuery] string? str)
        {
            var newCode = await _baseService.GenerateNewCodeUniqueFromStringValue(str);
            return Ok(newCode);
        }
        #endregion
    }
}
