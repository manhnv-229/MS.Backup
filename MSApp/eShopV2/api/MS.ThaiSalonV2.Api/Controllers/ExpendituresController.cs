﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.ApplicationCore.Entities;
using MS.ApplicationCore.Interfaces;
using System.Threading.Tasks;

namespace MS.ThaiSanlonV2.Api.Controllers
{
    public class ExpendituresController : BaseController<Expenditure>
    {
        IUnitOfWork _unitOfWork;
        public ExpendituresController(IExpenditureRepository repository, IExpenditureService baseService, IUnitOfWork unitOfWork) : base(repository, baseService)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("revenues")]
        public async Task<IActionResult> GetReceipts()
        {
            var revenues = await _unitOfWork.Expenditures.GetRevenues();
            return Ok(revenues);
        }

        [HttpGet("spends")]
        public async Task<IActionResult> GetPayments()
        {
            var expenditures = await _unitOfWork.Expenditures.GetExpenditures();
            return Ok(expenditures);
        }

        [HttpGet("general-info")]
        public async Task<IActionResult> GetInfo()
        {
            var expenditures = await _unitOfWork.Expenditures.GetGeneralInfo();
            return Ok(expenditures);
        }
    }
}
