using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using Template.Infrastructure.Cache.Contracts;

namespace Template.Infrastructure.Cache
{
    public class MemoryCacheProvider : ICacheProvider
    {
        private readonly IMemoryCache _cache;
        private readonly CacheSettings _settings;

        public MemoryCacheProvider(
            IMemoryCache memoryCache,
            IOptions<CacheSettings> settings
        )
        {
            _cache = memoryCache;
            _settings = settings.Value;
        }

        public bool TryGet<T>(string id, out T value)
        {
            if (_cache.TryGetValue(id, out value))
            {
                return true;
            }
            return false;
        }

        public T Set<T>(string key, T value)
        {
            if (value != null)
            {
                return _cache.Set(key, value, TimeSpan.FromMinutes(_settings.ExpiryTimeMinutes));
            }
            return default(T);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public T Set<T>(string key, T value, DateTimeOffset expiryDate)
        {
            if (value != null)
            {
                return _cache.Set(key, value, expiryDate);
            }
            return default(T);
        }
    }
}
