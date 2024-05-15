using Dapper;
using MS.ApplicationCore.DTOs;
using MS.ApplicationCore.Entities;
using MS.ApplicationCore.Interfaces.Repositories;
using MS.ApplicationCore.Utilities;
using MS.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace MS.Infrastructure.Data
{
    public class UnitRepostitory : DapperRepository<Unit>, IUnitRepository
    {
        public UnitRepostitory(MySqlDbContext sqlDbContext) : base(sqlDbContext)
        {
        }
    }
}
