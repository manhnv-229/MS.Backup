using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Interfaces
{
    public interface IAsyncService<TEntity>
    {
        Task<int> AddAsync(TEntity entity, IFormCollection? formData = null);
        Task<int> AddAsync(IEnumerable<TEntity> entities);
        Task<int> RemoveAsync(object key);
        Task<int> UpdateAsync(TEntity entity, object pks, IFormCollection? formData = null);
        Task<int> UpdateAsync(object entity, object pks = null);
    }
}
