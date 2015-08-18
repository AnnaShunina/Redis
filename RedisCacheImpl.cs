using System;

using StackExchange.Redis;

namespace ConsoleApplication1
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
        }


        private readonly string _name;
        private readonly Lazy<IDatabase> _database;


        private static IDatabase CreateDatabase(string connectionString)
        {
            var connection = ConnectionMultiplexer.Connect(connectionString);
            var database = connection.GetDatabase();
            return database;
        }


        public string Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key");
            }

            var cacheKey = GetCacheKey(key);

            return _database.Value.StringGet(cacheKey);
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


        private string GetCacheKey(string key)
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