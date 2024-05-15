using Dapper;
using MS.ApplicationCore.DTOs;
using MS.ApplicationCore.Entities;
using MS.ApplicationCore.Interfaces;
using MS.ApplicationCore.Utilities;
using MS.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Infrastructure.Data
{
    public class ProductRepository : DapperRepository<Product>, IProductRepository
    {
        public ProductRepository(MySqlDbContext sqlDbContext) : base(sqlDbContext)
        {
        }

        public async Task<IEnumerable<ProductSaleResponse>> GetProductSaleResponses()
        {
            var storeName = "Proc_Products_GetProductForSale";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@p_Organization", CommonFunction.OrganizationId);
            var products = await DbContext.Connection.QueryAsync<ProductSaleResponse>(storeName,parameters,transaction:DbContext.Transaction,commandType:System.Data.CommandType.StoredProcedure);
            return products;
        }
    }
}
