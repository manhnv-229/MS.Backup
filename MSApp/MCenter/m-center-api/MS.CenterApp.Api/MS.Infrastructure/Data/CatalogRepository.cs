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
    public class CatalogRepository : DapperRepository<Catalog>, ICatalogRepository
    {
        public CatalogRepository(IMSDatabaseContext dbContext) : base(dbContext)
        {
        }
    }
}
