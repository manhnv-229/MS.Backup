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
    public class PositionRepository : DapperRepository<EmployeePosition>, IPositionRepository
    {
        public PositionRepository(IMSDatabaseContext dbContext) : base(dbContext)
        {
        }
    }
}
