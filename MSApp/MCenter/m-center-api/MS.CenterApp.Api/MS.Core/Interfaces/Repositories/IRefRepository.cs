using MS.Core.DTOs;
using MS.Core.Entities;
using MS.Core.MSEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Interfaces.Repositories
{
    public interface IRefRepository : IRepository<Ref>
    {
        Task<string> GetNewRefNoAsync(Ref entity);

        Task<RefResponse?> GetRefById(Guid id);

        Task<PagingData> GetRefsByRefTypePaging(RefType? type, int limit, int offset, string? key);
    }
}
