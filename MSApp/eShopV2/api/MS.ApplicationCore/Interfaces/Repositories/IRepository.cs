using MS.ApplicationCore.DTOs;
using MS.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MS.ApplicationCore.Interfaces
{
    public interface IRepository<TEntity>:IAsyncRepository<TEntity> where TEntity : class
    {
        Task<bool> CheckDuplicateValue<TValidate>(string propName,dynamic propValue, TValidate entity) where TValidate:BaseEntity;
        Task<PagingData> GetFilterPaging(string[] columnsSort, string[] columlsFilter, string keyFilter, int limit, int offset);
        Task<PagingData> GetFilterPaging(string keyFilter, int limit, int offset, HashSet<ColumnSortInfo> columnSorts = null, HashSet<ColumnFilterInfo> columnFilters = null);
    }
}
