using AutoMapper;
using Dapper;
using MISA.FM.Infrastructure.Repositories;
using MS.Core.DTOs;
using MS.Core.Entities;
using MS.Core.Exceptions;
using MS.Core.Interfaces;
using MS.Core.MSEnums;
using MS.Core.Utilities;
using MS.Infrastructure.Interfaces;
using MS.Infrastructure.UnitOfWork;
using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static Dapper.SqlMapper;

namespace MS.Infrastructure.Data
{
    public class DapperRepository<TEntity> : IAsyncRepository<TEntity>, IRepository<TEntity> where TEntity : BaseEntity
    {
        //protected IUnitOfWork UnitOfWork = null;
        string _tableName = string.Empty;
        protected readonly IMSDatabaseContext DbContext;
        protected readonly IMapper _mapper;
        public DapperRepository(IMSDatabaseContext dbContext, IMapper mapper)
        {
            _tableName = typeof(TEntity).Name;
            //UnitOfWork = unitOfWork;
            DbContext = dbContext;
            _mapper = mapper;
        }
        #region Add
        protected void SetDefaultOrganization(dynamic entity)
        {
            var orgProp = entity.GetType().GetProperty("OrganizationId");
            if (orgProp != null && orgProp.GetValue(entity) == null && CommonFunction.OrganizationId != null)
            {
                // Nếu là dữ liệu hệ thống thì không cần gán OrgId:
                // ---> Kiểm tra xem dữ liệu hiện tại có là dữ liệu hệ thống hay không?
                var hasDefineSystem = DbContext.CheckTableHasSystemData<TEntity>();
                var hasIsSystemCol = DbContext.CheckColumnHasExistInTable(_tableName, "IsSystem");
                if (hasDefineSystem && hasIsSystemCol)
                {
                    var propIsSystem = entity.GetType().GetProperty("IsSystem");
                    var isSystemValue = false;
                    var type = propIsSystem?.PropertyType;
                    if (type == typeof(bool) || type == typeof(bool?) || type == typeof(Boolean) || type == typeof(Boolean?))
                    {
                        var value = propIsSystem?.GetValue(entity);
                        isSystemValue = (bool)(value == null ? false : value);
                    }
                    if (isSystemValue)
                    {
                        // Chỉ có Administrator được phép đặt dữ liệu hệ thống:
                        if (CommonFunction.MSRole == MSRole.Administrator)
                        {
                            orgProp.SetValue(entity, null);
                        }
                        else
                        {
                            propIsSystem?.SetValue(entity, false);
                            orgProp.SetValue(entity, Guid.Parse(CommonFunction.OrganizationId));
                        }
                    }
                    else
                    {
                        orgProp.SetValue(entity, Guid.Parse(CommonFunction.OrganizationId));
                    }
                }
                else
                {
                    orgProp.SetValue(entity, Guid.Parse(CommonFunction.OrganizationId));
                }
            }
        }
        public virtual async Task<int> AddAsync(dynamic entity, bool addChild = false)
        {
            SetDefaultOrganization(entity);
            if (entity is TEntity)
            {
                var rowAffects = await DbContext.AddAsync(entity, addChild);
                return rowAffects;
            }
            else
            {
                var entityMapper = _mapper.Map<TEntity>(entity);
                var rowAffects = await DbContext.AddAsync(entityMapper, addChild);
                return rowAffects;
                //var entityType = entity.GetType();
                //var method = DbContext.GetType()?.GetMethod("AddAsync")?.MakeGenericMethod(new Type[] { entityType });
                //var task = (Task?)method?.Invoke(this, new object[] { entity, addChild });

                //if (task != null)
                //{
                //    await task.ConfigureAwait(false);
                //    var resultProperty = task.GetType().GetProperty("Result");
                //    var rowAffects = resultProperty?.GetValue(task);
                //    return (int)rowAffects;
                //}
            }
            return 0;
        }

        public async Task<int> AddAsync(IEnumerable<TEntity> entities)
        {
            var res = 0;
            foreach (var item in entities)
            {
                res += await AddAsync(item);
            }
            return res;
        }
        #endregion

        #region GET

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            var sql = DbContext.BuildSelectAllQuery<TEntity>();
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@OrganizationId", CommonFunction.OrganizationId);
            return await DbContext.AppConnection.QueryAsync<TEntity>(sql, param: parameters, transaction: DbContext.Transaction, commandType: System.Data.CommandType.Text);

        }


        public virtual async Task<TEntity> GetByIdAsync(object pksFields)
        {
            return await DbContext.GetByIdAsync<TEntity>(pksFields.ToString());
        }

        #endregion

        #region Insert Or Update

        public virtual async Task<int> UpdateAsync(TEntity entity, object pks)
        {
            SetDefaultOrganization(entity);
            try
            {
                var rowAffects = await DbContext.UpdateAsync<TEntity>(entity);
                //var rowAffects = await DbContext.Connection.ExecuteAsync($"Proc_Update{_tableName}", entity, transaction: DbContext.Transaction, commandType: System.Data.CommandType.StoredProcedure);
                return rowAffects;
            }
            catch (Exception)
            {
                var rowAffects = await DbContext.UpdateAsync<TEntity>(entity);
                return rowAffects;
            }
        }

        public async Task<int> UpdateAsync(object entity, object? pks = null)
        {
            var propDics = JsonConvert.DeserializeObject<Dictionary<string, object>>(entity.ToString());
            DynamicParameters parameters = new DynamicParameters();
            if (pks != null)
            {
                parameters.Add($"@Id", pks);
            }
            var sqlQuery = DbContext.BuildUpdateSomePropertyQuery<TEntity>(propDics, parameters);
            var res = await DbContext.AppConnection.ExecuteAsync(sqlQuery, param: parameters, transaction: DbContext.Transaction);
            return res;

        }
        #endregion

        #region DELETE/REMOVE
        public virtual async Task<int> RemoveAsync(object key)
        {
            try
            {
                // Không cho phép người dùng thường xóa dữ liệu hệ thống:
                // ---> Kiểm tra xem dữ liệu hiện tại có là dữ liệu hệ thống hay không?
                var hasDefineSystem = DbContext.CheckTableHasSystemData<TEntity>();
                var hasIsSystemCol = DbContext.CheckColumnHasExistInTable(_tableName, "IsSystem");
                var role = CommonFunction.MSRole;
                if (hasDefineSystem && hasIsSystemCol && CommonFunction.MSRole != MSRole.Administrator)
                {
                    var obj = await GetByIdAsync(key);
                    var isSytem = (bool)obj?.GetType()?.GetProperty("IsSystem")?.GetValue(obj);
                    if (isSytem)
                    {
                        throw new MSException(HttpStatusCode.Forbidden, "Bạn không được phép xóa dữ liệu hệ thống!");
                    }
                }
                return await DbContext.DeleteAsync<TEntity>(key);
            }
            catch (MySqlException ex)
            {

                if (ex.Number == (int)MySqlErrorCode.RowIsReferenced2 || ex.Number == (int)MySqlErrorCode.RowIsReferenced)
                {
                    throw new MSMySqlException(HttpStatusCode.InternalServerError, "Không thể xóa do có phát sinh dữ liệu và nghiệp vụ liên quan.");
                }
                else
                {
                    throw;
                }
            }

        }

        /// <summary>
        /// Hàm kiểm tra xem giá trị của cột tương ứng trong database đã tồn tại hay chưa
        /// </summary>
        /// <param name="columnName">Tên cột</param>
        /// <param name="value">giá trị</param>
        /// <returns>true - đã tồn tại, false- giá trị chưa tồn tại</returns>
        /// CreatedBy: NVMANH (09/09/2022)
        public async Task<bool> CheckDuplicateValue<TValidate>(string columnName, dynamic value, TValidate entity) where TValidate : BaseEntity
        {
            var tableName = DbContext.GetTableName<TValidate>(entity);
            // Nếu là thêm mới:
            var sqlCommand = $"SELECT {columnName} FROM {tableName} WHERE {columnName} = @{columnName} AND OrganizationId = @OrganizationId";
            var parameters = new DynamicParameters();

            // Nếu là cập nhật:
            if (entity.EntityState == MSEntityState.UPDATE)
            {
                var primaryKey = DbContext.GetPrimaryKeyName<TValidate>();
                if (primaryKey != null)
                {
                    sqlCommand = $"SELECT e.{columnName} FROM {tableName} e WHERE e.{columnName} = @{columnName} AND {primaryKey} != @{primaryKey} AND OrganizationId = @OrganizationId";
                    parameters.Add($"@{primaryKey}", typeof(TValidate).GetProperty(primaryKey)?.GetValue(entity));
                }
                else
                {
                    throw new MSException(HttpStatusCode.InternalServerError, $"Không thể thực hiện cập nhật dữ liệu vào hệ thống. Vui lòng liên hê Mạnh Software để được giải quyết lỗi này.");
                }

            }

            parameters.Add($"@{columnName}", value);
            parameters.Add($"@OrganizationId", CommonFunction.OrganizationId);

            var record = await DbContext.AppConnection.QueryFirstOrDefaultAsync<object>(sqlCommand, param: parameters, transaction: DbContext.Transaction);
            if (record != null)
                return true;
            return false;
        }

        #endregion

        #region Other
        public virtual async Task<string> GetNewCodeDynamic()
        {
            return await DbContext.GetNewCodeDynamic<TEntity>();
        }

        public virtual async Task<PagingData> GetFilterPaging(string[] columnsSort, string[] columlsFilter, string keyFilter, int limit, int offset)
        {
            return await DbContext.GetFilterPaging<TEntity>(columnsSort, columlsFilter, keyFilter, limit, offset);
        }


        public async Task<PagingData> GetFilterPaging(string keyFilter, int limit, int offset, HashSet<ColumnSortInfo>? columnSorts = null, HashSet<ColumnFilterInfo>? columnFilters = null)
        {
            var columnsSortString = new List<string>();
            var columnFiltersString = new List<string>();
            if (columnSorts != null)
            {
                foreach (var col in columnSorts)
                {
                    var colName = col.ColumnName;
                    var sortType = col.SortType.ToString();
                    columnsSortString.Add($"{colName} {sortType}");
                }
            }

            if (columnFilters != null)
            {
                foreach (var col in columnFilters)
                {
                    var colName = col.ColumnName;
                    //var sortType = col.SortType.ToString();
                    columnFiltersString.Add($"{colName}");
                }
            }
            return await GetFilterPaging(columnsSortString.ToArray(), columnFiltersString.ToArray(), keyFilter, limit, offset);
        }

        public async Task<int> RemoveByOrganizationAsync(Guid organizationId)
        {
            var tableName = DbContext.GetTableName<TEntity>();
            var hasOrganizationId = DbContext.CheckColumnHasExistInTable(tableName, "OrganizationId");
            if (hasOrganizationId)
            {
                var sql = $"DELETE FROM {tableName} WHERE OrganizationId = @OrganizationId";
                var parameters = new DynamicParameters();
                parameters.Add("@OrganizationId", organizationId);
                var res = await DbContext.AppConnection.ExecuteAsync(sql, parameters, transaction: DbContext.Transaction);
                return res;
            }
            return 0;
        }


        #endregion

        #region Utilites
        protected async Task UpdateListByEntityState<TUpdate>(ICollection<TUpdate>? list) where TUpdate : BaseEntity
        {
            if (list != null && list.Count > 0)
            {
                // Xóa tất cả sản phẩm cũ trong giỏ hàng sau đó thêm giỏ mới vào:
                var productsDelete = list.Where(p => p.EntityState == Core.MSEnums.MSEntityState.DELETE);
                var productsUpdate = list.Where(p => p.EntityState == Core.MSEnums.MSEntityState.UPDATE);
                var productsAdd = list.Where(p => p.EntityState == Core.MSEnums.MSEntityState.ADD);
                //await DbContext.Transaction.BulkActionAsync(x => x.BulkDelete(productsDelete).ThenBulkUpdate(u => productsUpdate).ThenBulkInsert(i => productsAdd));

                foreach (var item in productsDelete)
                {
                    var keyName = DbContext.GetPrimaryKeyName<TUpdate>();
                    if (keyName != null)
                    {
                        var keyValue = item.GetType()?.GetProperty(keyName)?.GetValue(item);
                        _ = await DbContext.DeleteAsync<TUpdate>(keyValue);
                    }
                    else
                    {
                        await DbContext.DeleteAsync<TUpdate>(item);
                    }
                }

                foreach (var item in productsUpdate)
                {
                    await DbContext.UpdateAsync<TUpdate>(item);
                }

                foreach (var item in productsAdd)
                {
                    await DbContext.AddAsync<TUpdate>(item);
                }
            }
        }

        public async Task<string?> GetNewValueUnique<T>(string prefixCode, string? columnName = null) where T : class
        {
            var newValue = await DbContext.GetNewValueUnique<T>(prefixCode, columnName);
            return newValue;
        }
        #endregion
    }
}
