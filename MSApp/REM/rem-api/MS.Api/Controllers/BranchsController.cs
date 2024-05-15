using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.Core.Entities;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Repositories;
using MS.Core.Interfaces.Services;
using Ms.Api.Controllers;
using MS.Core.Authorization;

namespace Ms.Api.Controllers
{
    public class BranchsController : BaseController<Branch>
    {
        IBranchRepository _repository;
        IBranchService _service;
        public BranchsController(IBranchService service, IBranchRepository repository) : base(repository, service)
        {
            _service = service;
            _repository = repository;
        }

    }
}
