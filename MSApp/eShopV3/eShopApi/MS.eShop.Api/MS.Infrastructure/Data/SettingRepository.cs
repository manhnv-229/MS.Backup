using Microsoft.Extensions.Caching.Memory;
using MS.Core.Entities;
using MS.Core.Interfaces.Caches;
using MS.Core.Interfaces.Repositories;
using MS.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Infrastructure.Data
{
    public class SettingRepository : DapperRepository<Setting>, ISettingRepository
    {
        IMemoryCache _cache;
        public SettingRepository(IMSDatabaseContext dbContext, IMemoryCache cache) : base(dbContext)
        {
            _cache = cache;
        }

        public override async Task<int> AddAsync(Setting entity, bool addChild = false)
        {
            var res = await base.AddAsync(entity, addChild);
            var settingnews = await DbContext.GetAllAsync<Setting>();
            _cache.Set("Settings", settingnews);
            return res;
        }
        public override async Task<int> UpdateAsync(Setting entity, object pks)
        {
            var res = await base.UpdateAsync(entity, pks);
            var settingnews = await DbContext.GetAllAsync<Setting>();
            _cache.Set("Settings", settingnews);
            return res;
        }
    }
}
