using Dapper;
using Dapper.Contrib.Extensions;
using MISA.FM.Infrastructure.Repositories;
using MS.Core.DTOs;
using MS.Core.Entities;
using MS.Core.Interfaces.Repositories;
using MS.Core.Utilities;
using MS.Infrastructure.UnitOfWork;
using Mysqlx.Crud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;
using MS.Infrastructure.Interfaces;
using System.Data.Common;
using MS.Core.MSEnums;

namespace MS.Infrastructure.Data
{
    public class RefRepository : DapperRepository<Ref>, IRefRepository
    {
        public RefRepository(IMSDatabaseContext dbContext) : base(dbContext)
        {
        }

        public async override Task<PagingData> GetFilterPaging(string[] columnsSort, string[] columlsFilter, string keyFilter, int limit, int offset)
        {
            var storeName = "Proc_Ref_GetPaging";
            var totalRecords = 0;
            var parameters = new DynamicParameters();
            parameters.Add("@p_organizationId", CommonFunction.OrganizationId);
            parameters.Add("@p_limit", limit);
            parameters.Add("@p_offset", offset);
            parameters.Add("@p_textFilter", keyFilter);
            parameters.Add("@p_totalRecords", direction: System.Data.ParameterDirection.Output);
            var res = await DbContext.Connection.QueryAsync(storeName, parameters, commandType: System.Data.CommandType.StoredProcedure);
            totalRecords = parameters.Get<int>("@p_totalRecords");
            var pagingData = new PagingData()
            {
                TotalRecords = totalRecords,
                Data = res
            };
            return pagingData;
        }

        public override Task<int> AddAsync(Ref entity, bool addChild = true)
        {
            return base.AddAsync(entity, addChild);
        }
        public async override Task<IEnumerable<Ref>> GetAllAsync()
        {
            var sql = "SELECT * FROM Ref i ORDER BY i.CreatedDate DESC";
            var invoices = await DbContext.Connection.QueryAsync<Ref>(sql);
            return invoices;
        }

        public async override Task<int> UpdateAsync(Ref invoice, object pks)
        {
            var newTransaction = false;
            // Thêm mới Invoice:
            if (DbContext.Transaction == null)
            {
                newTransaction = true;
                DbContext.Transaction = DbContext.Connection.BeginTransaction();
            }

            var res = await DbContext.UpdateAsync(invoice);

            if (invoice.RefType == RefType.IN_WAREHOUSE || invoice.RefType == RefType.OUT_WAREHOUSE) {
                await UpdateListByEntityState<StockInventory>(invoice.StockInventory);
            }
            else
            {
                await UpdateListByEntityState<RefDetail>(invoice.RefDetail);
            }
            

            // Xoá các khoản nợ cũ và các khoản thanh toán cũ:
            var sqlDeleteDebitAndPayments = "DELETE FROM DebitDetail WHERE RefId = @RefId;" +
                "DELETE FROM InvoicePayment WHERE RefId = @RefId";
            await DbContext.Connection.ExecuteAsync(sqlDeleteDebitAndPayments, new { RefId = invoice.RefId }, transaction: DbContext.Transaction, commandType: System.Data.CommandType.Text);

            // Bổ sung khoản nợ mới:
            if (invoice.DebitDetail != null)
            {
                foreach (var item in invoice.DebitDetail)
                {
                    item.RefId = invoice.RefId;
                    await DbContext.AddAsync<DebitDetail>(item);
                }
            }

            // Bổ sung các khoản thanh toán mới:
            if (invoice.InvoicePayment != null)
            {
                foreach (var item in invoice.InvoicePayment)
                {
                    item.RefId = invoice.RefId;
                    await DbContext.AddAsync<InvoicePayment>(item);
                }
            }

            if (newTransaction)
            {
                DbContext.Transaction.Commit();
            }
            return res;
        }

        public async Task<string> GetNewRefNoAsync(Ref entity)
        {
            var storeName = "Proc_Ref_GetNewRefNo";
            var parameters = new DynamicParameters();
            parameters.Add("@p_organizationId", CommonFunction.OrganizationId);
            parameters.Add("@p_refType", entity.RefType);
            var res = await DbContext.Connection.QueryFirstOrDefaultAsync<string>(storeName, parameters, commandType: System.Data.CommandType.StoredProcedure);
            return res ?? "";
        }

        public async Task<RefResponse?> GetRefById(Guid id)
        {
            var storeName = "Proc_Ref_GetRefById";
            var parameters = new DynamicParameters();
            parameters.Add("@p_organizationId", CommonFunction.OrganizationId);
            parameters.Add("@p_refId", id);
            var sqlGetOrg = "SELECT * FROM Organization o WHERE o.OrganizationId = @p_organizationId";
            var res = await DbContext.Connection.QueryFirstOrDefaultAsync<RefResponse>(storeName, parameters, commandType: System.Data.CommandType.StoredProcedure);
            if (res != null)
            {
                res.Organization = await DbContext.Connection.QueryFirstOrDefaultAsync<Organization>(sqlGetOrg, param: parameters, commandType: System.Data.CommandType.Text);
                res.RefDetail = await DbContext.Connection.QueryAsync<RefDetailResponse>("Proc_Ref_GetRefDetailByRefId", parameters, commandType: System.Data.CommandType.StoredProcedure);
                res.StockInventory = await DbContext.Connection.QueryAsync<StockInventoryResponse>("Proc_Ref_GetStockInventoryByRefId", parameters, commandType: System.Data.CommandType.StoredProcedure);
                res.DebitDetail = await DbContext.Connection.QueryAsync<object>("Proc_Ref_GetDebitDetailByRefId", parameters, commandType: System.Data.CommandType.StoredProcedure);
                res.InvoicePayment = await DbContext.Connection.QueryAsync<object>("Proc_Ref_GetInvoicePaymentByRefId", parameters, commandType: System.Data.CommandType.StoredProcedure);
            }
            return res;
        }

        public async Task<PagingData> GetRefsByRefTypePaging(RefType? type, int limit, int offset, string? key)
        {
            var storeName = "Proc_Refs_GetRefsByRefTypePaging";
            var totalRecords = 0;
            var parameters = new DynamicParameters();
            parameters.Add("@p_refType", type);
            parameters.Add("@p_limit", limit);
            parameters.Add("@p_offset", offset);
            parameters.Add("@p_keySearch", key);
            parameters.Add("@p_organizationId", CommonFunction.OrganizationId);
            parameters.Add("@p_totalRecords", direction: System.Data.ParameterDirection.Output);
            var res = await DbContext.Connection.QueryAsync<object>(storeName, param: parameters, transaction: DbContext.Transaction, commandType: System.Data.CommandType.StoredProcedure);
            totalRecords = parameters.Get<int>("@p_totalRecords");
            var pagingData = new PagingData()
            {
                TotalRecords = totalRecords,
                Data = res
            };
            return pagingData;
        }
    }
}
