using Dapper;
using MISA.FM.Infrastructure.Repositories;
using MS.Core.DTOs;
using MS.Core.Entities;
using MS.Core.Interfaces.Repositories;
using MS.Core.Utilities;
using MS.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using MS.Infrastructure.Interfaces;

namespace MS.Infrastructure.Data
{
    public class UnitRepostitory : DapperRepository<Unit>, IUnitRepository
    {
        public UnitRepostitory(IMSDatabaseContext dbContext,AutoMapper.IMapper mapper) : base(dbContext,mapper)
        {
        }
    }
}
