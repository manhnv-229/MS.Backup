﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.ApplicationCore.Authorization;
using MS.ApplicationCore.Entities;
using MS.ApplicationCore.Interfaces;
using System;
using System.Threading.Tasks;

namespace MS.ThaiSanlonV2.Api.Controllers
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
