using Dapper;
using Dapper.Contrib.Extensions;
using MS.ApplicationCore.DTOs;
using MS.ApplicationCore.Entities;
using MS.ApplicationCore.Interfaces.Repositories;
using MS.ApplicationCore.Utilities;
using MS.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MS.Infrastructure.Data
{
    public class InvoiceRepository : DapperRepository<Invoice>,IInvoiceRepository
    {
        public InvoiceRepository(MySqlDbContext sqlDbContext) : base(sqlDbContext)
        {
        }

        public async override Task<PagingData> GetFilterPaging(string[] columnsSort, string[] columlsFilter, string keyFilter, int limit, int offset)
        {
            var storeName = "Proc_Invoice_GetPaging_V2";
            var totalRecords = 0;
            var parameters = new DynamicParameters();
            parameters.Add("@p_organizationId", CommonFunction.OrganizationId);
            parameters.Add("@p_limit", limit);
            parameters.Add("@p_offset", offset);
            parameters.Add("@p_textFilter", keyFilter);
            parameters.Add("@p_totalRecords", direction:System.Data.ParameterDirection.Output);
            var res = await DbContext.Connection.QueryAsync(storeName, parameters, commandType:System.Data.CommandType.StoredProcedure);
            totalRecords = parameters.Get<int>("@p_totalRecords");
            var pagingData = new PagingData()
            {
                TotalRecords = totalRecords,
                Data = res
            };
            return pagingData;
        }
        public async override Task<IEnumerable<Invoice>> GetAllAsync()
        {
            var sql = "SELECT * FROM Invoice i ORDER BY i.CreatedDate DESC";
            var invoices = await DbContext.Connection.QueryAsync<Invoice>(sql);
            return invoices;
        }

        public async override Task<int> UpdateAsync(Invoice invoice, object pks)
        {
            var newTransaction = false;
            // Thêm mới Invoice:
            if (DbContext.Transaction == null)
            {
                newTransaction = true;
                DbContext.Transaction = DbContext.Connection.BeginTransaction();
            }

            var res = await DbContext.UpdateAsync(invoice);

            // Xóa tất cả sản phẩm cũ trong giỏ hàng sau đó thêm giỏ mới vào:
            var productsDelete = invoice.InvoiceDetail.Where(p => p.EntityState == ApplicationCore.MSEnums.EntityState.DELETE);
            var productsUpdate = invoice.InvoiceDetail.Where(p => p.EntityState == ApplicationCore.MSEnums.EntityState.UPDATE);
            var productsAdd = invoice.InvoiceDetail.Where(p => p.EntityState == ApplicationCore.MSEnums.EntityState.ADD);
            //await DbContext.Transaction.BulkActionAsync(x => x.BulkDelete(productsDelete).ThenBulkUpdate(u => productsUpdate).ThenBulkInsert(i => productsAdd));

            foreach (var item in productsDelete)
            {
                await DbContext.DeleteAsync<InvoiceDetail>(item);
            }

            foreach (var item in productsUpdate)
            {
                await DbContext.UpdateAsync<InvoiceDetail>(item);
            }

            foreach (var item in productsAdd)
            {
                await DbContext.AddAsync<InvoiceDetail>(item);
            }
            if (newTransaction)
            {
                DbContext.Transaction.Commit();
            }
            return res;
        }
    }
}
