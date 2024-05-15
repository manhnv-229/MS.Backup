using Dapper;
using MISA.FM.Infrastructure.Repositories;
using MS.Core.DTOs;
using MS.Core.Entities;
using MS.Core.Interfaces;
using MS.Core.Utilities;
using MS.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;
using MS.Infrastructure.Interfaces;
using AutoMapper;

namespace MS.Infrastructure.Data
{
    public class InventoryItemRepository : DapperRepository<InventoryItem>, IInventoryItemRepository
    {
        public InventoryItemRepository(IMSDatabaseContext dbContext,AutoMapper.IMapper mapper) : base(dbContext, mapper)
        {
        }

        /// <summary>
        /// Danh sách hàng hoá - màn hình quản lý hàng hoá
        /// </summary>
        /// CreatedBy: NVMANH (25/10/2023)
        /// <returns></returns>
        public async Task<IEnumerable<InventoryItemResponse>> GetInventoryItemAdminResponses()
        {
            var storeName = "Proc_InventoryItems_GetInventoryItemAdminResponses";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@p_Organization", CommonFunction.OrganizationId);
            var inventoryItems = await DbContext.AppConnection.QueryAsync<InventoryItemResponse>(storeName, parameters, transaction: DbContext.Transaction, commandType: System.Data.CommandType.StoredProcedure);
            return inventoryItems;
        }

        public async Task<IEnumerable<InventoryItemSaleResponse>> GetInventoryItemSaleResponses()
        {
            var storeName = "Proc_InventoryItems_GetInventoryItemForSale";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@p_Organization", CommonFunction.OrganizationId);
            var products = await DbContext.AppConnection.QueryAsync<InventoryItemSaleResponse>(storeName, parameters, transaction: DbContext.Transaction, commandType: System.Data.CommandType.StoredProcedure);
            return products;
        }

        public async Task<IEnumerable<InventoryItemSaleResponse>> FilterInventoryItemSaleResponses(string key)
        {
            var storeName = "Proc_InventoryItems_FilterInventoryItemSaleResponses";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@p_Organization", CommonFunction.OrganizationId);
            parameters.Add("@p_key", key);
            var products = await DbContext.AppConnection.QueryAsync<InventoryItemSaleResponse>(storeName, parameters, transaction: DbContext.Transaction, commandType: System.Data.CommandType.StoredProcedure);
            return products;
        }

        public async Task<string> GetNewCodeAuto(string prefixCode)
        {
            var storeName = "Proc_InventoryItems_GetNewCodeAuto";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@p_prefixCode", prefixCode);
            parameters.Add("@p_OrganizationId", CommonFunction.OrganizationId);
            var newCode = await DbContext.AppConnection.QueryFirstOrDefaultAsync<string>(storeName, parameters, transaction: DbContext.Transaction, commandType: System.Data.CommandType.StoredProcedure);
            return newCode ?? string.Empty;
        }

        public async override Task<PagingData> GetFilterPaging(string[] columnsSort, string[] columlsFilter, string keyFilter, int limit, int offset)
        {
            var data =  await DbContext.GetFilterPaging<InventoryItemResponse>(columnsSort, columlsFilter, keyFilter, limit, offset);
            return data;
        }
    }
}
