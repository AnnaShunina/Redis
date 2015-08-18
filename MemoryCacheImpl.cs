﻿using System;
using System.Linq;
using System.Runtime.Caching;

namespace ConsoleApplication1
{
    internal class MemoryCacheImpl : ICache, IDisposable
    {
        public MemoryCacheImpl(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            _cache = new MemoryCache(name);
        }


        private readonly MemoryCache _cache;


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

            return (value == null);
        }

        public void Set(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            _cache.Set(key, value, DateTimeOffset.MaxValue);
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