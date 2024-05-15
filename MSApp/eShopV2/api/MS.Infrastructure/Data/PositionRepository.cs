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
    public class PositionRepository : DapperRepository<EmployeePosition>, IPositionRepository
    {
        public PositionRepository(MySqlDbContext sqlDbContext) : base(sqlDbContext)
        {
        }
    }
}
