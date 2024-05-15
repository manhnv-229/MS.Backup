using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using MS.Core.Entities;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Caches;
using MS.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Services
{
    public class SettingService : BaseService<Setting>, ISettingService
    {
        IMemoryCache _msMemoryCache;
        public SettingService(IRepository<Setting> repository, IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache msMemoryCache) : base(repository, unitOfWork, mapper)
        {
            _msMemoryCache = msMemoryCache;
        }
        protected async override Task AfterSave(dynamic entity, bool saveSuccess = true)
        {
            // Cập nhật lại cache:

            if (_msMemoryCache.TryGetValue("Settings", out IEnumerable<Setting> settings))
            {
                var setting = settings.Where(s=>s.SettingKey == entity.SettingKey).FirstOrDefault();
                if (setting != null)
                {
                    setting.SettingValue = entity.SettingValue;
                }
                else
                {
                    var newSettings = settings.ToList();
                    newSettings.Add(entity);
                    _msMemoryCache.Set("Settings", newSettings);
                }
            }
            else
            {
                settings = await UnitOfWork.Settings.GetAllAsync();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    //.SetSlidingExpiration(TimeSpan.FromSeconds(60))
                    //.SetAbsoluteExpiration(TimeSpan.FromSeconds(3600))
                    .SetPriority(CacheItemPriority.NeverRemove);
                //.SetSize(1024);
                _msMemoryCache.Set("Settings", settings, cacheEntryOptions);
            }
        }
    }
}
