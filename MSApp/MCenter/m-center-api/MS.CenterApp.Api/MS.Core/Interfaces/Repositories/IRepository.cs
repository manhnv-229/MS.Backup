using MS.Core.DTOs;
using MS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Interfaces
{
    public interface IRepository<TEntity>:IAsyncRepository<TEntity> where TEntity : class
    {
        Task<string?> GetNewValueUnique<T>(string prefixCode, string? columnName = null) where T : class;
        Task<bool> CheckDuplicateValue<TValidate>(string propName,dynamic propValue, TValidate entity) where TValidate:BaseEntity;
        Task<PagingData> GetFilterPaging(string[] columnsSort, string[] columlsFilter, string keyFilter, int limit, int offset);
        Task<PagingData> GetFilterPaging(string keyFilter, int limit, int offset, HashSet<ColumnSortInfo>? columnSorts = null, HashSet<ColumnFilterInfo> columnFilters = null);
    }
}
