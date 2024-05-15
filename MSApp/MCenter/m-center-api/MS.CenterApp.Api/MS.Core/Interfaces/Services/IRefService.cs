using MS.Core.DTOs;
using MS.Core.Entities;
using MS.Core.MSEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Interfaces.Services
{
    public interface IRefService : IBaseService<Ref>
    {
        Task<PagingData> GetRefsByRefTypePaging(RefType? type, int limit, int offset, string? key);
    }
}
