using System;
using System.Linq;
using Redis.Cache.Interface;
using StackExchange.Redis;

namespace Redis.Cache
{
    internal class RedisCacheImpl : ICache, IDisposable
    {
        public RedisCacheImpl(string name, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            _name = name;
            _database = new Lazy<IDatabase>(() => CreateDatabase(connectionString));
        }


        private readonly string _name;
        private readonly Lazy<IDatabase> _database;

        private static IDatabase CreateDatabase(string connectionString)
        {
            var connection = ConnectionMultiplexer.Connect(connectionString);
            var database = connection.GetDatabase();
            return database;
        }


        public bool Contains(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key");
            }

            var cacheKey = GetCacheKey(key);

            return _database.Value.KeyExists(cacheKey);
        }

        public string Get(string key)
        {
            string value;

            TryGet(key, out value);

            return value;
        }

        public bool TryGet(string key, out string value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key");
            }

            var cacheKey = GetCacheKey(key);
            var cacheValue = _database.Value.StringGet(cacheKey);

            value = cacheValue;

            return cacheValue.HasValue;
        }

        public void Set(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key");
            }

            var cacheKey = GetCacheKey(key);

            if (value != null)
            {
                _database.Value.StringSet(cacheKey, value);
            }
            else
            {
                _database.Value.KeyDelete(cacheKey);
            }
        }

        public void Remove(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key");
            }
            
            var cacheKey = GetCacheKey(key);

            _database.Value.KeyDelete(cacheKey);
        }

        public void Clear()
        {
            var database = _database.Value;

            var endpoints = database.Multiplexer.GetEndPoints();

            if (endpoints != null && endpoints.Length > 0)
            {
                var server = database.Multiplexer.GetServer(endpoints[0]);

                if (server != null)
                {
                    var allKeys = server.Keys(database.Database, _name + ".*").ToArray();

                    database.KeyDelete(allKeys);
                }
            }
        }


        private string GetCacheKey(string key)
        {
            return string.Format("{0}.{1}", _name, key);
        }


        public void Dispose()
        {
            if (_database.IsValueCreated)
            {
                var connection = _database.Value.Multiplexer;
                connection.Dispose();
            }
        }
    }
}