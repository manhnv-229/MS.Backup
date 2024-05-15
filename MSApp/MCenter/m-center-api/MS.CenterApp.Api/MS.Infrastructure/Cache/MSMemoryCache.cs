using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Caching.Memory;
using MS.Core.Entities;
using MS.Core.Interfaces.Caches;
using MS.Infrastructure.Data;
using MS.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace MS.Infrastructure.Cache
{
    public class MSMemoryCache 
    {
        protected readonly IMemoryCache _iMSCache;
        //protected readonly List<Nationality> Nationalities;
        //protected readonly List<Relation> Relations;
        //protected readonly List<Ethnic> Ethnics;
        protected readonly List<Setting> Settings;
        //protected List<Position> Positions;
        //protected List<Department> Departments;

        public MSMemoryCache(IMemoryCache iMSCache)
        {
            _iMSCache = iMSCache;
        }

        /// <summary>
        /// Thực hiện cache dữ liệu các danh mục phục vụ cho import dữ liệu:
        /// </summary>
        /// CreatedBy: NVMANH (05/2020)
        public void CacheGetOrCreate()
        {
            SetCache("Settings", Settings);
            //SetCache("Nationalities", Nationalities);
            //SetCache("Relations", Relations);
            //SetCache("Ethnics", Ethnics);
            //SetCache("Positions", Positions);
            //SetCache("Departments", Departments);
        }

        /// <summary>
        /// Lấy cache dữ liệu Theo key
        /// </summary>
        /// CreatedBy: NVMANH (05/2020)
        public object GetCache(string key)
        {
            var cacheEntry = _iMSCache.Get<object>(key);
            return cacheEntry;
        }

        /// <summary>
        ///  Thực hiện cache dữ liệu
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="data">Dữ liệu cache</param>
        /// CreatedBy: NVMANH (25/05/2020)
        public void SetCache(string key, object data, double timeExpiration = 0)
        {
            _iMSCache.GetOrCreate(key, entry =>
            {
                if (timeExpiration > 0)
                {
                    entry.SlidingExpiration = TimeSpan.FromMinutes(timeExpiration);
                }
                else
                {
                    entry.Priority = CacheItemPriority.NeverRemove;
                }

                return data;
            });
        }
    }
}
