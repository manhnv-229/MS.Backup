using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using static Dapper.SqlMapper;
using MS.Core.Exceptions;
using MS.Core.DTOs;
using MS.Core.Utilities;
using MS.Core.Entities;
using MS.Core.Helpers;
using MS.Core.MSEnums;
using MS.Infrastructure.Interfaces;

namespace MISA.FM.Infrastructure.Repositories
{
    public class MariaDbContext : IDisposable, IMSDatabaseContext
    {
        protected readonly string ConnectionString;
        public IDbConnection Connection { get; }
        public IDbTransaction Transaction { get; set; }
        public MariaDbContext(IConfiguration configuration, IWebHostEnvironment _env)
        {
            try
            {
                if (_env.IsDevelopment())
                {
                    ConnectionString = configuration.GetConnectionString("MSDevConnection");
                }
                else if (_env.IsProduction())
                {
                    ConnectionString = configuration.GetConnectionString("MSProConnection");
                }
                else
                {
                    ConnectionString = configuration.GetConnectionString("MSConnection");
                }
                Connection = new MySqlConnection(ConnectionString);
                Connection.Open();
                Console.WriteLine("Connection is Openned!!!");
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Connection is Error!!!");
                if (ex.ErrorCode == MySqlErrorCode.UnableToConnectToHost)
                {
                    throw new MSMySqlException(System.Net.HttpStatusCode.InternalServerError, "Không thể kết nối đến dữ liệu hệ thống. Liên hệ quản trị viên để được trợ giúp.");
                }
                else
                {
                    throw;
                }

            }
        }

        public string GetConnectionString()
        {
            return ConnectionString;
        }
        public void Dispose()
        {
            Console.WriteLine("Connection is Closed!!!");
            Connection?.Dispose();
        }

        #region Methods

        public async Task<string> GetNewCodeDynamic<T>() where T : class
        {
            var storeName = "Proc_GetNewCode";
            var tableName = GetTableName<T>();
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@TableName", tableName);
            parameters.Add("@p_OrganizationId", CommonFunction.OrganizationId);
            var newCode = await Connection.QueryFirstOrDefaultAsync<string>(storeName, parameters, transaction: Transaction, commandType: CommandType.StoredProcedure);
            return newCode;
        }

        public IEnumerable<PropertyInfo> GetListPropertyInfoIsCollection<T>(T entity)
        {
            var props = typeof(T).GetProperties();
            if (entity != null)
            {
                props = entity.GetType().GetProperties();
            }

            // Kiểm tra xem entiy có danh sách chi tiết đi kèm không? nếu có thì add luôn vào db:
            var collections = typeof(T).GetProperties().Where(prop => (prop.PropertyType.IsGenericType && typeof(ICollection<>).IsAssignableFrom(prop.PropertyType.GetGenericTypeDefinition())));
            return collections;
        }

        /// <summary>
        ///  Lấy các Property là kiểu tham chiếu (được định nghĩa từ class - VD Organization trong class Product)
        /// </summary>
        /// <typeparam name="T">Kiểu của Object</typeparam>
        /// <param name="entity">Đối tượng được khởi tạo từ object</param>
        /// <returns>Danh sách propperty</returns>
        /// CreatedBy: NVMANH (26/11/2022)
        public IEnumerable<PropertyInfo> GetListPropertyInfoIsPrimitives<T>(T entity)
        {
            var props = typeof(T).GetProperties();
            var propInfos = new List<PropertyInfo>();
            if (entity != null)
            {
                props = entity.GetType().GetProperties();
            }

            // Kiểm tra xem entiy có danh sách chi tiết đi kèm không? nếu có thì add luôn vào db:
            var cols = props.Where(prop => prop.PropertyType.IsClass && prop.PropertyType != typeof(string));
            return cols;
        }

        private void GetParentsInfoReferenceToEntity<T>(T entity)
        {
            var colIsPrimitives = GetListPropertyInfoIsPrimitives(entity);
            foreach (var prop in colIsPrimitives)
            {
                var propTypeName = prop.PropertyType.Name;
                var referenceTableId = $"{propTypeName}Id";
                var foreinKeyAttribute = prop.GetCustomAttribute(typeof(MSForeignKey), false);
                if (foreinKeyAttribute != null)
                {
                    referenceTableId = (foreinKeyAttribute as MSForeignKey).ForeignKeyName;
                }
                
                var referenceTableValue = entity.GetType().GetProperty(referenceTableId)?.GetValue(entity);
                if (referenceTableValue != null)
                {
                    var sqlSelectReferenceObject = $"SELECT * FROM {propTypeName} t WHERE t.{referenceTableId} = '{referenceTableValue.ToString()}'";

                    MethodInfo method = GetType().GetMethod("QueryFirstOrDefault")?
                               .MakeGenericMethod(new Type[] { prop.PropertyType });

                    var referenceObject = method?.Invoke(this, new object[] { sqlSelectReferenceObject, null });
                    prop.SetValue(entity, referenceObject);
                }
            }
        }

        public virtual async Task<IReadOnlyList<T>> GetAllAsync<T>() where T : class
        {
            var tableName = GetTableName<T>();
            var sql = $"select * from {tableName}";
            var colsInDb = GetColumnsInDbOfTable(tableName);
            var organizationId = colsInDb.FirstOrDefault(c => c.ColumnName == "OrganizationId")?.ColumnName;
            var notGetByOrg = typeof(T).IsDefined(typeof(DoNotGetByOranization));
            DynamicParameters parameters = new DynamicParameters();
            if (organizationId != null && !notGetByOrg)
            {
                var whereCondition = $"WHERE 1=1";
                var hasSystemData = CheckTableHasSystemData<T>();
                var isSysTemProp = CheckColumnHasExistInTable(tableName, "IsSystem");
                if (hasSystemData && isSysTemProp)
                {
                    whereCondition += $" AND (OrganizationId = @OrganizationId OR OrganizationId IS NULL OR IsSystem = 1)";
                }
                else
                {
                    whereCondition += $" AND OrganizationId = @OrganizationId";
                }
                sql = $"select * from {tableName} t WHERE {whereCondition}";
                parameters.Add("@OrganizationId", CommonFunction.OrganizationId);
            }
            var data = await Connection.QueryAsync<T>(sql, param: parameters, transaction: Transaction);
            return data.ToList();
        }

        public virtual async Task<T> GetByIdAsync<T>(object id)
        {
            var tableName = GetTableName<T>();
            var propKeyName = GetPrimaryKeyName<T>();

            var sqlCommand = $"SELECT * FROM {tableName} WHERE {propKeyName} = @Id";

            var colsInDb = GetColumnsInDbOfTable(tableName);
            var organizationId = colsInDb.FirstOrDefault(c => c.ColumnName == "OrganizationId")?.ColumnName;
            var notGetByOrg = typeof(T).IsDefined(typeof(DoNotGetByOranization));
            DynamicParameters parameters = new DynamicParameters();


            if (organizationId != null && organizationId != propKeyName && !notGetByOrg)
            {
                sqlCommand = $"SELECT * FROM {tableName} WHERE {propKeyName} = @Id AND (OrganizationId = @p_OrganizationId OR OrganizationId IS NULL)";
                parameters.Add("@p_OrganizationId", CommonFunction.OrganizationId);
            }
            parameters.Add($"@Id", id);
            var entity = await QueryFirstOrDefaultAsync<T>(sqlCommand, parameters);


            // Lấy các đối tượng được tham chiếu đến:
            GetParentsInfoReferenceToEntity(entity);

            // Nếu có bản ghi được chọn thì lấy tiếp dữ liệu bảng detail:
            var propIsCollections = GetListPropertyInfoIsCollection<T>(entity);
            foreach (var propIsCollection in propIsCollections)
            {
                var detailTableName = propIsCollection.PropertyType.GetGenericArguments().FirstOrDefault()?.Name;
                var genericAgrs = propIsCollection.PropertyType.GetGenericArguments().FirstOrDefault();

                //var detailType = genericAgrs.MakeGenericType();
                //var generic = detailType.MakeGenericType(detailType);

                var sqlSelectDetail = $"SELECT * FROM {detailTableName} WHERE {propKeyName} = @Id";

                MethodInfo method = GetType().GetMethod("Query")?
                            .MakeGenericMethod(new Type[] { genericAgrs });

                var details = method?.Invoke(this, new object[] { sqlSelectDetail, parameters });

                //var details = await Connection.QueryAsync<object>(sqlSelectDetail, param: parameters, transaction: Transaction);
                propIsCollection.SetValue(entity, details);
            }
            return entity;
        }

        public IEnumerable<T> Query<T>(string sql, object parameters) where T : class
        {
            var data = Connection.Query<T>(sql, param: parameters, transaction: Transaction);
            foreach (var item in data)
            {
                GetParentsInfoReferenceToEntity<T>(item);
            }
            return data;
        }

        public T QueryFirstOrDefault<T>(string sql, object parameters)
        {
            var data = Connection.QueryFirstOrDefault<T>(sql, param: parameters, transaction: Transaction);
            return data;
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object parameters)
        {
            var data = await Connection.QueryFirstOrDefaultAsync<T>(sql, param: parameters, transaction: Transaction);
            return data;
        }

        public async Task<PagingData> GetFilterPaging<T>(string[] columnsSort, string[] columlsFilter, string keyFilter, int limit, int offset, string? tableName = null)
        {
            var pagingData = new PagingData();
            var notGetByOrg = typeof(T).IsDefined(typeof(DoNotGetByOranization));
            if (tableName == null)
            {
                // Lấy từ View, không khai báo View thì lấy thẳng trong table:
                var viewData = GetViewNameData<T>();
                if (viewData == null)
                {
                    tableName = GetTableName<T>();
                }
                else
                {
                    tableName = viewData;
                }
            }
            var columnSortString = "CreatedDate DESC";
            if (columnsSort.Length > 0)
            {
                columnSortString = string.Join(",", columnsSort);
            }
            var hasOrgId = CheckColumnHasExistInTable(tableName, "OrganizationId");
            var orgId = CommonFunction.OrganizationId;


            var whereCondition = $"WHERE 1=1";
            var limitOffsetCondition = "";

            if (limit > 0)
            {
                limitOffsetCondition = " LIMIT @limit OFFSET @offset";
            }

            if (hasOrgId && !notGetByOrg)
            {
                var hasSystemData = CheckTableHasSystemData<T>();
                var isSysTemProp = CheckColumnHasExistInTable(tableName, "IsSystem");
                if (hasSystemData && isSysTemProp)
                {
                    whereCondition += $" AND (OrganizationId = @OrganizationId OR OrganizationId IS NULL OR IsSystem = 1)";
                }
                else
                {
                    whereCondition += $" AND OrganizationId = @OrganizationId";
                }
            }
            if (columlsFilter.Length > 0 && !string.IsNullOrEmpty(keyFilter))
            {
                var colCondition = new HashSet<string>();
                foreach (var column in columlsFilter)
                {
                    colCondition.Add($"{column} LIKE CONCAT('%',@keyFilter,'%')");
                }

                whereCondition += $" AND ({string.Join(" OR ", colCondition)})";
            }


            var sql = $"CREATE TEMPORARY TABLE IF NOT EXISTS tbTemp AS " +
                         $"SELECT ROW_NUMBER() OVER(ORDER BY {columnSortString}) AS RowIndex, " +
                         $"i.* FROM {tableName} i " +
                         $"{whereCondition};" +
                         $"SELECT COUNT(*) AS TotalRecords FROM tbTemp;" +
                         $"SELECT * FROM tbTemp {limitOffsetCondition};";
            var parameters = new DynamicParameters();
            parameters.Add("@OrganizationId", orgId);
            parameters.Add("@keyFilter", keyFilter);
            parameters.Add("@limit", limit);
            parameters.Add("@offset", offset);
            //var res = await Connection.QueryMultipleAsync(sql, parameters, transaction: Transaction);
            using (var multi = await Connection.QueryMultipleAsync(sql, parameters, transaction: Transaction))
            {
                pagingData.TotalRecords = multi.ReadFirstOrDefault<int>();
                pagingData.Data = multi.Read<object>();
            }
            return pagingData;
        }

        public async Task<PagingData> GetFilterPaging<T>(string keyFilter, int limit, int offset, HashSet<ColumnSortInfo>? columnSorts = null, HashSet<ColumnFilterInfo>? columnFilters = null, string? tableName = null)
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
            return await GetFilterPaging<T>(columnsSortString.ToArray(), columnFiltersString.ToArray(), keyFilter, limit, offset, tableName);
        }

        public virtual async Task<int> AddAsync<T>(T entity, bool addChild = false) where T : BaseEntity
        {
            var sqlCommand = BuildAddQuery(entity);

            var newTransaction = false;
            if (Transaction == null)
            {
                newTransaction = true;
                Transaction = Connection.BeginTransaction();
            }
            // Thêm Master trước:
            var rowMasterAdd = await Connection.ExecuteAsync(sqlCommand, param: entity, transaction: Transaction);

            // Nếu addChild = true thì thực hiện Duyệt các Collection con đã lấy được để thêm vào:
            if (addChild == true)
            {
                var propKeyMasterName = GetPrimaryKeyName<T>();
                if (propKeyMasterName != null)
                {
                    var propKeyMasterValue = entity.GetType().GetProperty(propKeyMasterName)?.GetValue(entity);
                    // Kiểm tra xem entiy có danh sách chi tiết đi kèm không? nếu có thì add luôn vào db:
                    var collections = typeof(T).GetProperties().Where(prop => (prop.PropertyType.IsGenericType && typeof(ICollection<>).IsAssignableFrom(prop.PropertyType.GetGenericTypeDefinition())))?.ToHashSet();
                    if (collections != null && rowMasterAdd > 0 && collections.Count > 0)
                    {
                        foreach (var collection in collections)
                        {
                            // Chú ý nếu convert sang ICollection thì lỗi item sẽ null -> sửa lại convert là IEnumerable
                            var items = collection.GetValue(entity) as IEnumerable; // as ICollection;

                            // Duyệt từng item và thêm vào database - chú ý là bổ sung giá trị khóa phụ (nếu có) của master:
                            //var sql = "";
                            if (items != null)
                            {
                                foreach (var item in items)
                                {
                                    if (propKeyMasterValue != null)
                                    {
                                        var propForeinKeyWithMaster = item.GetType().GetProperty(propKeyMasterName);
                                        if (propForeinKeyWithMaster != null)
                                        {
                                            propForeinKeyWithMaster.SetValue(item, propKeyMasterValue);
                                        }
                                    }
                                    SetNewGuidForPrimaryKey(item);

                                    //sql = BuildAddQuery<dynamic>(item);
                                    //await Connection.ExecuteAsync(sql, items, transaction: Transaction);

                                    var genericAgrs = item.GetType();

                                    //gọi đệ quy AddAssync để thêm liên tục các bản ghi con, cháu, chắt của master
                                    MethodInfo? method = GetType().GetMethod("AddAsync")?
                                                .MakeGenericMethod(new Type[] { genericAgrs });


                                    var task = (Task?)method?.Invoke(this, new object[] { item, addChild });

                                    if (task != null)
                                    {
                                        await task.ConfigureAwait(false);
                                        var resultProperty = task.GetType().GetProperty("Result");
                                        var add = resultProperty?.GetValue(task);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (newTransaction)
            {
                Transaction.Commit();
            }
            return rowMasterAdd;
        }

        public virtual async Task<int> UpdateAsync<T>(T entity, bool updateChild = false) where T : BaseEntity
        {
            var sqlCommand = BuildUpdateQuery(entity);
            if (sqlCommand == null)
                throw new MSException(System.Net.HttpStatusCode.InternalServerError, "Có lỗi khi thực hiện cập nhật dữ liệu vào hệ thống, liên hệ Mạnh Software để được giải quyết!");

            var rowUpdated = await Connection.ExecuteAsync(sqlCommand, param: entity, transaction: Transaction);
            if (updateChild == true)
            {
                var propKeyMasterName = GetPrimaryKeyName<T>();
                if (propKeyMasterName == null)
                {
                    return rowUpdated;
                }
                var propKeyMasterValue = entity.GetType().GetProperty(propKeyMasterName)?.GetValue(entity);
                // Kiểm tra xem entiy có danh sách chi tiết đi kèm không? nếu có thì add luôn vào db:
                var collections = typeof(T).GetProperties().Where(prop => (prop.PropertyType.IsGenericType && typeof(ICollection<>).IsAssignableFrom(prop.PropertyType.GetGenericTypeDefinition())))?.ToHashSet();
                if (collections != null && collections.Count > 0)
                {
                    foreach (var collection in collections)
                    {
                        var items = collection.GetValue(entity) as IEnumerable;

                        // Duyệt từng item Kiểm tra trạng thái (EntityState) từng item và thực hiện cập nhật, thêm hoặc xóa
                        if (items != null)
                        {
                            var foreinKeyAttribute = collection.GetCustomAttribute(typeof(MSForeignKey), false);
                            var foreinKeyName = propKeyMasterName;
                            // Kiểm tra xem có gán MSForeignKey cho thuộc tính hay không, nếu có thì sử dụng tên thuộc tính này:
                            if (foreinKeyAttribute != null)
                            {
                                var masterTableName = (foreinKeyAttribute as MSForeignKey)?.MasterTableName;
                                // Kiểm tra có gán master table name không, không có thì lấy luôn tên của class hiện tại:
                                if (masterTableName == null)
                                    masterTableName = GetTableName<T>();

                                // Tương tự kiểm tra tiếp forein key name:
                                foreinKeyName = (foreinKeyAttribute as MSForeignKey)?.ForeignKeyName;
                                if (foreinKeyName == null)
                                    foreinKeyName = propKeyMasterName;

                                var hasExits = CheckColumnHasExistInTable(masterTableName, foreinKeyName);
                                // Kiểm tra tên cột foreinKey, có trong bảng ở db thì mới sử dụng, không có phát báo lỗi ngay:
                                if (!hasExits)
                                {
                                    throw new MSMySqlException(System.Net.HttpStatusCode.InternalServerError, "Có lỗi xảy ra khi thực hiện cập nhật dữ liệu, vui lòng liên hệ Mạnh Software để được trợ giúp.");
                                }
                            }

                            // Xử lý tên của khoá chính, khoá ngoại xong thì tiếp tục xử lý từng Item trong danh sách:
                            foreach (var item in items)
                            {
                                var entityStateProp = item.GetType().GetProperty("EntityState");
                                var propType = entityStateProp?.PropertyType;

                                if (entityStateProp != null && item != null && (propType == typeof(MSEntityState?) || propType == typeof(MSEntityState)))
                                {
                                    var entityStateValue = entityStateProp?.GetValue(item);
                                    item.GetType().GetProperty(foreinKeyName)?.SetValue(item, propKeyMasterValue);

                                    if (entityStateValue != null)
                                    {
                                        switch ((MSEntityState)entityStateValue)
                                        {

                                            case MSEntityState.ADD:
                                                // Nếu item ở trạng thái thêm mới thì tạo Id mới cho item:
                                                SetNewGuidForPrimaryKey(item);
                                                // Sau đó thực hiện thêm mới vào Db:
                                                var add = await InvokeAddAsync(item, updateChild);
                                                break;
                                            case MSEntityState.UPDATE:
                                                var update = await InvokeUpdateAsync(item, updateChild);
                                                break;
                                            case MSEntityState.DELETE:
                                                var delete = await InvokeDeleteAsync(item, updateChild);
                                                break;
                                            default:
                                                break;
                                        }

                                    }
                                }




                            }
                        }
                    }
                }
            }
            return rowUpdated;
        }

        public virtual async Task<int> DeleteAsync<T>(object id, bool deleteChil = false) where T : BaseEntity
        {
            var sqlCommand = BuildDeleteQueryById<T>();
            var propKeyName = GetPrimaryKeyName<T>();
            var parameters = new DynamicParameters();
            parameters.Add($"@{propKeyName}", id);
            var rowsDeleted = await Connection.ExecuteAsync(sqlCommand, param: parameters, transaction: Transaction);
            return rowsDeleted;
        }

        //public virtual async Task<int> DeleteAsync<T>(T entity, bool deleteChil = false) where T : BaseEntity
        //{
        //    var tableName = GetTableName<T>(entity);
        //    var sqlCommand = BuildDeleteQueryById<T>();
        //    //var propKeyName = GetPrimaryKeyName<T>();
        //    //var parameters = new DynamicParameters();
        //    //if (propKeyName!= null)
        //    //{
        //    //    var id = entity.GetType().GetProperty(propKeyName)?.GetValue(entity, null);
        //    //    parameters.Add($"@{propKeyName}", id);
        //    //}
        //    var rowsDeleted = await Connection.ExecuteAsync(sqlCommand, param: entity, transaction: Transaction);
        //    return rowsDeleted;
        //}
        #endregion

        #region Utilities
        public string GetTableName<T>()
        {
            var tableAttr = typeof(T).GetCustomAttributes(typeof(DataTableName), true).FirstOrDefault();
            if (tableAttr != null)
            {
                return (tableAttr as DataTableName).Name;
            }
            else
            {
                return typeof(T).Name;
            }
        }
        public string GetTableName<T>(T entity)
        {
            var tableAttr = entity.GetType().GetCustomAttributes(typeof(DataTableName), true).FirstOrDefault();
            if (tableAttr != null)
            {
                return (tableAttr as DataTableName).Name;
            }
            else
            {
                return entity.GetType().Name;
            }
        }

        /// <summary>
        /// Lấy tên của View Data được khai báo trong class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// CreatedBy: NVMANH (25/03/2024)
        public string? GetViewNameData<T>()
        {
            var viewName = typeof(T).GetCustomAttributes(typeof(ViewDataName), true).FirstOrDefault();
            if (viewName != null)
            {
                return (viewName as ViewDataName).Name;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Lấy danh sách các cột của bảng được tạo trong database:
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private IEnumerable<MSDbColumn> GetColumnsInDbOfTable(string tableName)
        {
            var storeName = "Proc_GetAllColumnOfTable";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@p_TableName", tableName);
            parameters.Add("@p_DbName", Connection.Database);
            var data = Connection.Query<MSDbColumn>(storeName, param: parameters, transaction: Transaction, commandType: CommandType.StoredProcedure);
            return data;
        }

        /// <summary>
        /// Kiểm tra bảng hiện tại có lữu trữ dữ liệu dạng hệ thống (không cho phép xóa)
        /// </summary>
        /// <typeparam name="T">Bảng dữ liệu</typeparam>
        /// <returns>true - có định nghĩa; false - không</returns>
        /// CreatedBy: NVMANH (30/12/2022)
        public bool CheckTableHasSystemData<T>()
        {
            return typeof(T).IsDefined(typeof(HasSystemData));
        }
        /// <summary>
        /// Kiểm tra xem cột dữ liệu có trong table hay không
        /// </summary>
        /// <param name="tableName">tên bảng cần kiểm tra</param>
        /// <param name="columnName">tên cột cần kiểm tra</param>
        /// <returns>true - có trong table, false nếu không có trong table</returns>
        /// CreatedBy: NVMANH (20.11.2022)
        public bool CheckColumnHasExistInTable(string tableName, string columnName)
        {
            var cols = GetColumnsInDbOfTable(tableName);
            var colExist = cols.FirstOrDefault(c => c.ColumnName == columnName);
            return colExist != null;
        }

        private void SetNewGuidForPrimaryKey<T>(T entity)
        {
            var primaryKeyName = GetPrimaryKeyName<T>(entity);
            var prop = entity?.GetType().GetProperty(primaryKeyName);
            var propValue = prop?.GetValue(entity);
            // ĐẶt giá trị mặc định cho khóa chính - trong trường hợp chưa được gán giá trị
            if (prop != null && (prop.PropertyType == typeof(Guid) || prop.PropertyType == typeof(Guid?)) && (propValue == null || propValue.ToString() == Guid.Empty.ToString()))
            {
                prop.SetValue(entity, Guid.NewGuid());
            }
        }
        public string? GetPrimaryKeyName<T>()
        {
            var tableName = GetTableName<T>();
            var colsInDb = GetColumnsInDbOfTable(tableName);
            var propKeyName = colsInDb.FirstOrDefault(c => c.ColumnKey == "PRI")?.ColumnName;
            return propKeyName;
        }


        private string? GetPrimaryKeyName<T>(T entity)
        {
            var tableName = entity?.GetType().Name;
            if (tableName != null)
            {
                var colsInDb = GetColumnsInDbOfTable(tableName);
                var propKeyName = colsInDb.FirstOrDefault(c => c.ColumnKey == "PRI")?.ColumnName;
                if (propKeyName == null)
                {
                    propKeyName = GetPrimaryKeyName<T>();
                }
                return propKeyName;
            }
            else
            {
                return null;
            }

        }
        public async Task<string?> GetNewValueUnique<T>(string prefixCode, string? columnName = null) where T : class
        {

            var tableName = GetTableName<T>();
            if (columnName == null)
            {
                columnName = $"{tableName}Code";
            }
            var exits = CheckColumnHasExistInTable(tableName, columnName);
            if (exits == false)
            {
                return null;
            }
            var storeName = "Proc_GetNewValueUnique";
            var parameters = new DynamicParameters();
            parameters.Add("@prefixCode", prefixCode);
            parameters.Add("@columnName", columnName);
            parameters.Add("@tableName", tableName);
            parameters.Add("@orgId", CommonFunction.OrganizationId);
            var res = await Connection.ExecuteScalarAsync<string>(storeName, parameters, commandType: CommandType.StoredProcedure);
            return res;
        }
        #region BuildQueryString
        public string BuildSelectAllQuery<T>()
        {
            var tableName = GetViewNameData<T>();
            if (tableName == null)
            {
                tableName = GetTableName<T>();
            }
            var sql = $"SELECT * FROM {tableName}";
            var orgColumnHasExits = CheckColumnHasExistInTable(tableName, "OrganizationId");
            if (orgColumnHasExits)
            {
                var whereCondition = $"WHERE 1=1";
                var hasSystemData = CheckTableHasSystemData<T>();
                var isSysTemProp = CheckColumnHasExistInTable(tableName, "IsSystem");
                if (hasSystemData && isSysTemProp)
                {
                    whereCondition += $" AND (OrganizationId = @OrganizationId OR OrganizationId IS NULL OR IsSystem = 1)";
                }
                else
                {
                    whereCondition += $" AND OrganizationId = @OrganizationId";
                }
                sql = $"SELECT * FROM {tableName} t {whereCondition}";
            }
            return sql;
        }

        /// <summary>
        /// Build chuỗi câu truy vấn thêm mới;
        /// </summary>
        /// <param name="entity">đối tượng thêm mới</param>
        /// <returns>chuỗi câu truy vấn INSERT</returns>
        /// Author: NVMANH (11/08/2022)
        public string BuildAddQuery<T>(T entity)
        {
            if (entity == null)
                return "";
            var tableName = GetTableName<T>(entity);

            var colCommandList = string.Empty;
            var colCommandParamList = string.Empty;

            var colsInDb = GetColumnsInDbOfTable(tableName);
            foreach (var col in colsInDb)
            {
                var colName = col.ColumnName;
                var primaryKey = (col.ColumnKey == "PRI");
                if (primaryKey)
                {
                    var prop = entity.GetType().GetProperty(colName);
                    var propValue = prop?.GetValue(entity);
                    // ĐẶt giá trị mặc định cho khóa chính - trong trường hợp chưa được gán giá trị
                    if (prop != null && (prop.PropertyType == typeof(Guid) || prop.PropertyType == typeof(Guid?)) && (propValue == null || propValue.ToString() == Guid.Empty.ToString()))
                    {
                        prop.SetValue(entity, Guid.NewGuid());
                    }
                }
                colCommandList = String.Join(",", colCommandList, colName);
                colCommandParamList = String.Join(",", colCommandParamList, $"@{colName}");
            }
            colCommandList = colCommandList.Substring(1, colCommandList.Length - 1);
            colCommandParamList = colCommandParamList.Substring(1, colCommandParamList.Length - 1);
            var sql = $"INSERT INTO {tableName}({colCommandList}) VALUES ({colCommandParamList})";
            return sql;
        }

        /// <summary>
        /// Build chuỗi câu truy vấn sửa;
        /// </summary>
        /// <param name="entity">đối tượng thêm mới</param>
        /// <returns>chuỗi câu truy vấn UPDATE</returns>
        /// Author: NVMANH (11/08/2022)
        public string? BuildUpdateQuery<T>(T entity)
        {
            if (entity != null)
            {
                var tableName = GetTableName<T>();
                var colCommandList = string.Empty;
                var propKeyName = $"{tableName}Id";
                var colsInDb = GetColumnsInDbOfTable(tableName);
                foreach (var col in colsInDb)
                {
                    var colName = col.ColumnName;
                    var primaryKey = (col.ColumnKey == "PRI");
                    if (primaryKey)
                    {
                        propKeyName = colName;
                        continue;
                    }
                    var prop = typeof(T).GetProperty(colName);
                    var propValue = prop?.GetValue(entity);
                    colCommandList = String.Join(",", colCommandList, $"{colName}=@{colName}");
                }

                colCommandList = colCommandList.Substring(1, colCommandList.Length - 1);
                var sql = $"UPDATE {tableName} SET {colCommandList} WHERE {propKeyName} = @{propKeyName}";
                return sql;
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// Build câu lệnh Update một số trường
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyValuePairs"></param>
        /// <returns>Chuỗi câu truy vấn</returns>
        /// CreatedBy: NVMANH (27/12/2022)
        public string BuildUpdateSomePropertyQuery<T>(Dictionary<string, object> keyValuePairs, DynamicParameters parameters, object? pks = null) where T : class
        {
            var tableName = GetTableName<T>();
            var propKeyName = GetPrimaryKeyName<T>();
            var colQueryUpdate = "";
            foreach (var item in keyValuePairs)
            {
                var colName = item.Key;
                var colValue = item.Value;
                if (colName == propKeyName)
                {
                    if (pks == null)
                        parameters.Add($"@Id", colValue);
                    continue;
                }
                else
                {
                    // Kiểm tra xem cột hiện tại có trong table của db hay ko, có thì mới build lệnh Update:
                    var hasExits = CheckColumnHasExistInTable(tableName, colName);

                    if (hasExits)
                    {
                        colQueryUpdate = String.Join(",", colQueryUpdate, $"{colName}=@{colName}");
                        parameters.Add($"@{colName}", colValue);
                    }

                }
            }
            colQueryUpdate = colQueryUpdate.Substring(1, colQueryUpdate.Length - 1);
            var sql = $"UPDATE {tableName} SET {colQueryUpdate} WHERE {propKeyName} = @Id";
            return sql;
        }

        /// <summary>
        /// Build chuỗi câu sql xóa dữ liệu theo khóa chính;
        /// </summary>
        /// <returns>chuỗi câu truy vấn DELETE</returns>
        /// Author: NVMANH (11/08/2022)
        public string BuildDeleteQueryById<T>()
        {
            var tableName = GetTableName<T>();
            var propKeyName = GetPrimaryKeyName<T>();
            return $"DELETE FROM {tableName} WHERE {propKeyName} = @{propKeyName}";
        }
        #endregion

        #endregion


        #region Invoke
        private async Task<object?> InvokeAddAsync(object item, bool updateChild)
        {
            // Sau đó thực hiện thêm mới vào Db:
            var genericAgrs = item.GetType();

            //gọi đệ quy AddAssync để thêm liên tục các bản ghi con, cháu, chắt của master
            MethodInfo? method = GetType().GetMethod("AddAsync")?
                        .MakeGenericMethod(new Type[] { genericAgrs });


            var task = (Task?)method?.Invoke(this, new object[] { item, updateChild });

            if (task != null)
            {
                await task.ConfigureAwait(false);
                var resultProperty = task.GetType().GetProperty("Result");
                var add = resultProperty?.GetValue(task);
                return add;
            }
            return null;
        }

        private async Task<object?> InvokeUpdateAsync(object item, bool updateChild)
        {
            // Sau đó thực hiện thêm mới vào Db:
            var genericAgrs = item.GetType();

            //gọi đệ quy AddAssync để thêm liên tục các bản ghi con, cháu, chắt của master
            MethodInfo? method = GetType().GetMethod("UpdateAsync")?
                        .MakeGenericMethod(new Type[] { genericAgrs });


            var task = (Task?)method?.Invoke(this, new object[] { item, updateChild });

            if (task != null)
            {
                await task.ConfigureAwait(false);
                var resultProperty = task.GetType().GetProperty("Result");
                var add = resultProperty?.GetValue(task);
                return add;
            }
            return null;
        }

        private async Task<object?> InvokeDeleteAsync(object item, bool deleteChild)
        {
            // Sau đó thực hiện thêm mới vào Db:
            var genericAgrs = item.GetType();

            var tableName = genericAgrs.Name;
            var colsInDb = GetColumnsInDbOfTable(tableName);
            var propKeyName = colsInDb.FirstOrDefault(c => c.ColumnKey == "PRI")?.ColumnName;
            if (propKeyName != null)
            {
                var propKeyValue = item.GetType()?.GetProperty(propKeyName?.ToString())?.GetValue(item);
                //gọi đệ quy AddAssync để thêm liên tục các bản ghi con, cháu, chắt của master
                MethodInfo? method = GetType().GetMethod("DeleteAsync")?
                            .MakeGenericMethod(new Type[] { genericAgrs });

                if (propKeyValue != null)
                {
                    var task = (Task?)method?.Invoke(this, new object[] { propKeyValue, deleteChild });

                    if (task != null)
                    {
                        await task.ConfigureAwait(false);
                        var resultProperty = task.GetType().GetProperty("Result");
                        var delete = resultProperty?.GetValue(task);
                        return delete;
                    }
                }

            }
            return null;
        }
        #endregion
    }
}
