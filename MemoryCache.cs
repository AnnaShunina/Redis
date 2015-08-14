using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemoryCache;

namespace ConsoleApplication1
{
    internal class MemoryCache : ICache, IDisposable
    {
        public MemoryCache()
        {
        }

        public string Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key");
            }

            return Cache.Get(key).ToString();
        }

        public void Set(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key");
            }

            if (value != null)
            {
                Cache.Store(key,value);
            }
            else
            {
                Cache.Remove(key);
            }
        }

        public void Set(string key, string value, TimeSpan timemout)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key");
            }

            if (timemout <= TimeSpan.Zero)
            {
                Set(key, value);
            }
            else
            {
                if (value != null)
                {
                    Cache.Store(key,value,timemout.Milliseconds);
                }
                else
                {
                    Cache.Remove(key);
                }
            }
        }

        public void Dispose()
        {
            Cache.Flush();
        }

    }
}
