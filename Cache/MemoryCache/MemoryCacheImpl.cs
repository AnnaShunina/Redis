using System;
using System.Linq;
using System.Runtime.Caching;
using Redis.Cache.Interface;

namespace Redis.Cache.MemoryCache
{
    internal class MemoryCacheImpl : ICache, IDisposable
    {
        public MemoryCacheImpl(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            _cache = new System.Runtime.Caching.MemoryCache(name);
            Name = name;
        }


        private readonly System.Runtime.Caching.MemoryCache _cache;
        public string Name { get; private set; }

        public bool Contains(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            return _cache.Contains(key);
        }

        public string Get(string key)
        {
            string value;

            TryGet(key, out value);

            return value;
        }

        public bool TryGet(string key, out string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            value = (string)_cache.Get(key);

            return value != null;
        }

        public void Set(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key");
            }

            if (value != null)
            {
                _cache.Set(key, value, new CacheItemPolicy());
            }
            else
            {
                _cache.Remove(key);
            }

            
        }

        public void Remove(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            _cache.Remove(key);
        }

        public void Clear()
        {
            foreach (var item in _cache.ToArray())
            {
                _cache.Remove(item.Key);
            }
        }

        public void Dispose()
        {
            _cache.Dispose();
        }
    }
}