using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Interfaces
{
    public interface IAsyncRepository<TEntity> where TEntity : class
    {
        Task<string> GetNewCodeDynamic();
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetByIdAsync(object pksFields);
        Task<int> AddAsync(TEntity entity, bool addChild = false);
        Task<int> AddAsync(IEnumerable<TEntity> entities);
        Task<int> RemoveAsync(object key);
        Task<int> UpdateAsync(TEntity entity, object pks = null);
        Task<int> UpdateAsync(object entity, object pks = null);
        Task<int> RemoveByOrganizationAsync(Guid organizationId);
    }
}
