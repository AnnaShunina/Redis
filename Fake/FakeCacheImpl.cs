using System;
using System.Collections.Generic;
using Redis.Cache.Interface;

namespace Redis.Fake
{
    internal sealed class FakeCacheImpl : ICache
    {
        public readonly Dictionary<string, string> Data
            = new Dictionary<string, string>();


        public bool Contains(string key)
        {
            return Data.ContainsKey(key);
        }

        public string Get(string key)
        {
            string value;
            if (Data.TryGetValue(key, out value)) return value;
            return null;
        }

        public bool TryGet(string key, out string value)
        {
            return Data.TryGetValue(key, out value);
        }

        public void Set(string key, string value)
        {
            Data[key] = value;
        }

        public void Remove(string key)
        {
            Data.Remove(key);
        }

        public void Clear()
        {
            Data.Clear();
        }
    }
}