using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.Core.Authorization;
using MS.Core.Entities;
using MS.Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace Ms.eShop.Api.Controllers
{
    public class PicturesController : BaseController<Picture>
    {
        IPictureRepository _pictureRepository;
        public PicturesController(IPictureRepository repository, IPictureService service) : base(repository, service)
        {
            _pictureRepository= repository;
        }
        [AllowAnonymous]
        [HttpPut("{id}/total-views")]
        public async Task<IActionResult> Put(Guid id)
        {
            var res = await _pictureRepository.UpdateTotalViewPicture(id);
            return Ok(res);
        }
    }
}
