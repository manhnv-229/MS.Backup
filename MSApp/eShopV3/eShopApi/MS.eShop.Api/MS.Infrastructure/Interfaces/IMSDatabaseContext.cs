using Dapper;
using MS.Core.DTOs;
using MS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MS.Infrastructure.Interfaces
{
    public interface IMSDatabaseContext
    {
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; set; }
        string GetConnectionString();
        Task<string> GetNewCodeDynamic<T>() where T : class;
        IEnumerable<PropertyInfo> GetListPropertyInfoIsCollection<T>(T entity);
        IEnumerable<PropertyInfo> GetListPropertyInfoIsPrimitives<T>(T entity);
        Task<IReadOnlyList<T>> GetAllAsync<T>() where T : class;
        Task<T> GetByIdAsync<T>(object id);
        IEnumerable<T> Query<T>(string sql, object parameters) where T : class;
        T QueryFirstOrDefault<T>(string sql, object parameters);
        Task<T> QueryFirstOrDefaultAsync<T>(string sql, object parameters);
        Task<PagingData> GetFilterPaging<T>(string[] columnSorts, string[] columnFilters, string keyFilter, int limit, int offset, string? tableName = null);
        Task<PagingData> GetFilterPaging<T>(string keyFilter, int limit, int offset, HashSet<ColumnSortInfo>? columnSorts = null, HashSet<ColumnFilterInfo>? columnFilters = null, string? tableName = null);
        Task<int> AddAsync<T>(T entity, bool addChild = false) where T : BaseEntity;
        Task<int> UpdateAsync<T>(T entity, bool updateChild = false) where T : BaseEntity;
        Task<int> DeleteAsync<T>(object id, bool deleteChil = false) where T : BaseEntity;
        string GetTableName<T>();
        string GetTableName<T>(T entity);
        bool CheckTableHasSystemData<T>();
        bool CheckColumnHasExistInTable(string tableName, string columnName);
        string? GetPrimaryKeyName<T>();
        string BuildSelectAllQuery<T>();
        string? BuildAddQuery<T>(T entity);
        string? BuildUpdateQuery<T>(T entity);
        string BuildUpdateSomePropertyQuery<T>(Dictionary<string, object> keyValuePairs, DynamicParameters parameters, object? pks = null) where T : class;
        string BuildDeleteQueryById<T>();
    }
}
