using System;
using Microsoft.Extensions.Caching.Memory;

namespace PBXDashboard_Dev.Models
{
    public class CurrentCallList<TItem>
    {
        private MemoryCache _cache = new MemoryCache(new MemoryCacheOptions()
        {
            SizeLimit = 2048
        });
        public TItem GetOrCreate(object key, Func<TItem> creatItem)
        {
            TItem cacheEntry;
            if (!_cache.TryGetValue(key, out cacheEntry))
            {
                _cache.Set(key, cacheEntry);
            }
            return cacheEntry;
        }
    }
}