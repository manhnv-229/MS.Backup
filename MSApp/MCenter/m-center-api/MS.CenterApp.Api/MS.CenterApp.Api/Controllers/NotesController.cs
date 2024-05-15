using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.CenterApp.Api.Controllers;
using MS.Core.Entities;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Repositories;
using MS.Core.Interfaces.Services;

namespace MS.eShop.Api.Controllers
{
    public class NotesController : BaseController<Note>
    {
        INoteService _noteService;
        INoteRepository _noteRepository;
        public NotesController(INoteRepository repository, INoteService service) : base(repository, service)
        {
            _noteService = service;
            _noteRepository = repository;
        }

        public async override Task<IActionResult> Post([FromBody] Note entity, [FromForm] IFormCollection? formData = null)
        {
            var res = await _noteService.AddNewNote(entity);
            return Ok(entity);
        }
    }
}
