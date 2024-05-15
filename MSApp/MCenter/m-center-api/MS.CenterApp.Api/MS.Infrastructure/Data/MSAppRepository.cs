using MS.Core.Entities;
using MS.Core.Interfaces.Repositories;
using MS.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Infrastructure.Data
{
    public class MSAppRepository : DapperRepository<MSApp>, IMSAppRepository
    {
        public MSAppRepository(IMSDatabaseContext dbContext) : base(dbContext)
        {
        }
    }
}
