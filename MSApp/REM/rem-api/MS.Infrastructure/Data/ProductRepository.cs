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
using MS.Infrastructure.Interfaces;

namespace MS.Infrastructure.Data
{
    public class ProductRepository : DapperRepository<Product>, IProductRepository
    {
        public ProductRepository(IMSDatabaseContext dbContext,AutoMapper.IMapper mapper) : base(dbContext,mapper)
        {
        }

        public async Task<IEnumerable<ProductSaleResponse>> GetProductSaleResponses()
        {
            var storeName = "Proc_Products_GetProductForSale";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@p_Organization", CommonFunction.OrganizationId);
            var products = await DbContext.AppConnection.QueryAsync<ProductSaleResponse>(storeName,parameters,transaction:DbContext.Transaction,commandType:System.Data.CommandType.StoredProcedure);
            return products;
        }
    }
}
