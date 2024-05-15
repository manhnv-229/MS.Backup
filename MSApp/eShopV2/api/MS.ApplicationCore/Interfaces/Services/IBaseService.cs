using MS.ApplicationCore.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MS.ApplicationCore.Interfaces
{
    public interface IBaseService<TEntity>:IAsyncService<TEntity>
    {
        Task<PagingData> GetFilterPaging(string keyFilter, int limit, int offset,HashSet<ColumnSortInfo> columnSortInfos = null, HashSet<ColumnFilterInfo> columnFilterInfos = null);
    }
}
