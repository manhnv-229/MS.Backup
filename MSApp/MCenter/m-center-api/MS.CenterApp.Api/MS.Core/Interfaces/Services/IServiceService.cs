using Microsoft.AspNetCore.Http;
using MS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Interfaces.Services
{
    public interface IServiceService:IBaseService<MS.Core.Entities.Service>
    {
        Task<int> AddNewService(Service service, IFormFile imgFile);
    }
}
