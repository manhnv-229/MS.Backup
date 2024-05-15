using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Interfaces.Caches
{
    public interface IMSMemoryCache
    {
        object GetCache(string key);
        void CacheGetOrCreate();
        void SetCache(string key, object data, double timeExpiration = 0);
    }
}
