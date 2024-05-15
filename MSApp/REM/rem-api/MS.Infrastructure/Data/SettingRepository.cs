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
        public SettingRepository(IMSDatabaseContext dbContext, IMemoryCache cache,AutoMapper.IMapper mapper) : base(dbContext,mapper)
        {
            _cache = cache;
        }

        public override async Task<int> AddAsync(dynamic entity, bool addChild = false)
        {
            var setting = entity as Setting;
            var res = await base.AddAsync(setting, addChild);
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
