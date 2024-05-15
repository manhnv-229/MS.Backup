using Dapper;
using MISA.FM.Infrastructure.Repositories;
using MS.Core.Entities;
using MS.Core.Interfaces.Repositories;
using MS.Core.Utilities;
using MS.Infrastructure.Interfaces;
using MS.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace MS.Infrastructure.Data
{
    public class CustomerRepository : DapperRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(IMSDatabaseContext dbContext,AutoMapper.IMapper mapper) : base(dbContext,mapper)
        {
        }

        public async override Task<IEnumerable<Customer>> GetAllAsync()
        {
            var customers = new List<Customer>();
            var sql = "SELECT * FROM Customer";
            customers = (await DbContext.AppConnection.QueryAsync<Customer>(sql, transaction: DbContext.Transaction))?.ToList();
            if (customers != null)
            {
                customers.Insert(0, new Customer
                {
                    CustomerId = Guid.Empty,
                    FullName = "(Khách vãng lai)"
                });
                return customers;
            }
            else
                return customers;
        }

    }
}
