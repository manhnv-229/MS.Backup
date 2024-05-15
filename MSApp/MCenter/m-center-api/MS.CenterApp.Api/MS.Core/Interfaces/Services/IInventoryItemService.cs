using Microsoft.AspNetCore.Http;
using MS.Core.Entities;
using MS.Core.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Interfaces.Services
{
    public interface IInventoryItemService : IBaseService<InventoryItem>
    {
        Task<int> AddNewService(InventoryItem user, IFormFile avatarFile);

        Task<string> GetNewCodeAuto(string name);
    }
}
