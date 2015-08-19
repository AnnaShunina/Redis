using System;
using System.Collections.Generic;
using StackExchange.Redis;

namespace Redis
{
    internal class RedisCacheImpl : ICache, IDisposable
    {
        public RedisCacheImpl(string name, string connectionString)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            _name = name;
            _database = new Lazy<IDatabase>(() => CreateDatabase(connectionString));
            _listValues = new List<string>();
        }


        private readonly string _name;
        private readonly Lazy<IDatabase> _database;
        private readonly List<string> _listValues;  

        private static IDatabase CreateDatabase(string connectionString)
        {
            var connection = ConnectionMultiplexer.Connect(connectionString);
            var database = connection.GetDatabase();
            return database;
        }


        public bool Contains(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            return _database.Value.KeyExists(GetCacheKey(key));
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

            value = (string)_database.Value.StringGet(GetCacheKey(key));

            if (!_listValues.Contains(GetCacheKey(key)))
            {
                _listValues.Add(GetCacheKey(key));
            }

            return (value == null);
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

                if (!_listValues.Contains(cacheKey))
                {
                    _listValues.Add(cacheKey);
                }
            }
            else
            {
                _database.Value.KeyDelete(cacheKey);
                _listValues.Remove(cacheKey);
            }
        }

        public void Remove(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            _database.Value.KeyDelete(GetCacheKey(key));
            _listValues.Remove(GetCacheKey(key));
        }

        public void Clear()
        {
            foreach (var item in _listValues)
            {
                _database.Value.KeyDelete(item);
            }
            _listValues.Clear();
        }


        public string GetCacheKey(string key)
        {
            return string.Format("{0}.{1}", _name, key);
        }


        public void Dispose()
        {
            if (_database.IsValueCreated)
            {
                var connection = _database.Value.Multiplexer;
                connection.Close();
                connection.Dispose();
            }
        }
    }
}