using MS.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Interfaces
{
    public interface IBaseService<TEntity>:IAsyncService<TEntity>
    {
        Task<string?> GenerateNewCodeUniqueFromStringValue(string stringValue, string? columnName = null);
        Task<PagingData> GetFilterPaging(string keyFilter, int limit, int offset,HashSet<ColumnSortInfo>? columnSortInfos = null, HashSet<ColumnFilterInfo>? columnFilterInfos = null);
    }
}
