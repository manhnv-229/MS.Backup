using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ms.Api.Controllers;
using MS.Core.Entities;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Repositories;
using MS.Core.Interfaces.Services;
using Newtonsoft.Json;

namespace MS.Api.Controllers
{
    public class ServicesController : BaseController<MS.Core.Entities.Service>
    {
        IServiceService _service;
        IServiceRepository _repository;
        public ServicesController(IServiceService service, IServiceRepository repository) : base(repository, service)
        {
            _service = service;
            _repository = repository;
        }

        public async override Task<IActionResult> Post([FromBody] Service entity, IFormCollection? formData = null)
        {
            // need to check if it is actually a form content type, as formModel may be bound to an empty instance
            var service = new Service();
            if (Request.HasFormContentType && formData != null)
            {
                service = JsonConvert.DeserializeObject<MS.Core.Entities.Service>(formData["service"].First());
                var file = formData.Files?.FirstOrDefault();
                var res = await _service.AddNewService(service, file);
                return Ok(res);
            }
            return await base.Post(entity, formData);
        }
    }
}
