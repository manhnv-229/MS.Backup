using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.ApplicationCore.Authorization;
using MS.ApplicationCore.Entities;
using MS.ApplicationCore.Interfaces;
using MS.ApplicationCore.MSEnums;
using System.Threading.Tasks;

namespace MS.ThaiSanlonV2.Api.Controllers
{
    public class ExpenditurePlansController : BaseController<ExpenditurePlan>
    {
        IDictionaryEnumService _dictionaryEnumService;
        IUnitOfWork _unitOfWork;
        public ExpenditurePlansController(IRepository<ExpenditurePlan> repository, IBaseService<ExpenditurePlan> baseService, IDictionaryEnumService dictionaryEnumService, IUnitOfWork unitOfWork) : base(repository, baseService)
        {
            _dictionaryEnumService = dictionaryEnumService;
            _unitOfWork = unitOfWork;
        }

        [AllowAnonymous]
        [HttpGet("plan-type")]
        public IActionResult GetExpenditurePlanTypeList([FromQuery] ReceiptType? type)
        {
            return Ok(_dictionaryEnumService.GetExpenditurePlanType(type));
        }

        public async override Task<IActionResult> Get(string key, int limit, int offset)
        {
            return Ok(await _unitOfWork.ExpenditurePlans.GetExpenditurePlans());
        }

        [HttpGet("filter")]
        public async Task<IActionResult> Filter([FromQuery] int? type)
        {
            return Ok(await _unitOfWork.ExpenditurePlans.GetExpenditurePlans(type));
        }
    }
}
