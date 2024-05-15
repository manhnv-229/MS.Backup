using MS.ApplicationCore.Entities;
using MS.ApplicationCore.Interfaces.Repositories;
using MS.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Infrastructure.Data
{
    public class CustomerRepository : DapperRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(MySqlDbContext sqlDbContext) : base(sqlDbContext)
        {
        }
    }
}
