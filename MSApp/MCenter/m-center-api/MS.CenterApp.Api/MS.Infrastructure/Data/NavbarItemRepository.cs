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
    public class NavbarItemRepository : DapperRepository<NavbarItem>, INavbarItemRepository
    {
        public NavbarItemRepository(IMSDatabaseContext dbContext) : base(dbContext)
        {
        }

        public async override Task<IEnumerable<NavbarItem>> GetAllAsync()
        {
            var data =  await base.GetAllAsync();
            var masters = data.Where(n => n.ParentId == null);
            masters = masters.OrderBy(item => item.Sort);
            foreach (var master in masters)
            {
                var key = master.NavbarItemId;
                var childs = data.Where((n) => n.ParentId == key);
                childs = childs.OrderBy(item => item.Sort);
                master.NavbarSubItems = childs.ToList();
            }
            return masters;
        }
    }
}
