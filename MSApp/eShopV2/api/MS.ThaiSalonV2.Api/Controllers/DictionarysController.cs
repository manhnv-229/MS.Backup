using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.ApplicationCore.Authorization;
using MS.ApplicationCore.Interfaces;
using MS.ApplicationCore.MSEnums;

namespace MS.ThaiSanlonV2.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DictionarysController : ControllerBase
    {
        IDictionaryEnumService _dictionaryEnumService;
        public DictionarysController(IDictionaryEnumService dictionaryEnumService)
        {
            _dictionaryEnumService = dictionaryEnumService;
        }

        [AllowAnonymous]
        [HttpGet("plan-type")]
        public IActionResult GetExpenditurePlanTypeList([FromQuery] ReceiptType? type)
        {
            var res = _dictionaryEnumService.GetExpenditurePlanType(type);
            return Ok(res);
        }

        [AllowAnonymous]
        [HttpGet("expenditure-type")]
        public IActionResult GetExpenditureTypes([FromQuery] ReceiptType? type)
        {
            var res = _dictionaryEnumService.GetExpenditureTypes(type);
            return Ok(res);
        }

        [AllowAnonymous]
        [HttpGet("gender")]
        public IActionResult GetGenders()
        {
            return Ok(_dictionaryEnumService.GetGenders());
        }

        [AllowAnonymous]
        [HttpGet("work-status")]
        public IActionResult GetWorkStatus()
        {
            return Ok(_dictionaryEnumService.GetWorkStatus());
        }

        [AllowAnonymous]
        [HttpGet("time-range")]
        public IActionResult GetTimeRange()
        {
            return Ok(_dictionaryEnumService.GetTimeRange());
        }

        [AllowAnonymous]
        [HttpGet("vip-options")]
        public IActionResult GetVIPOptions()
        {
            return Ok(_dictionaryEnumService.GetVIPOptions());
        }
    }
}
