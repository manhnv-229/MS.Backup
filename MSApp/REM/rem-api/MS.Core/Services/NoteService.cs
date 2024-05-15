using AutoMapper;
using MS.Core.Entities;
using MS.Core.Exceptions;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Repositories;
using MS.Core.Interfaces.Services;
using MS.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Services
{
    public class NoteService : BaseService<Note>, INoteService
    {
        public NoteService(INoteRepository repository, IUnitOfWork unitOfWork, IMapper mapper) : base(repository, unitOfWork, mapper)
        {
        }


        protected async override Task BeforeSave(dynamic entity)
        {
            if (entity.EntityState == MSEnums.MSEntityState.ADD)
            {
                entity.NoteId = Guid.NewGuid();
                if (CommonFunction.UserId != null)
                {
                    entity.UserId = Guid.Parse(CommonFunction.UserId);
                }
                entity.NoteDateTime = CommonFunction.ConvertDateToVNTime(DateTime.Now);
            }
            else if (entity.EntityState == MSEnums.MSEntityState.UPDATE)
            {
                entity.NoteDateTime = CommonFunction.ConvertDateToVNTime(DateTime.Now);
            }
        }

        protected async override Task AfterSave(dynamic entity, bool saveSuccess = true)
        {
        }

        public async Task<Note> AddNewNote(Note note)
        {
            var res = await AddAsync(note);
            if (res > 0)
            {
                return note;
            }
            else
            {
                throw new MSException(System.Net.HttpStatusCode.InternalServerError, "Không thể thực hiện thêm mới ghi chú");
            }
        }
    }
}
