using System;
using StackExchange.Redis;

namespace ConsoleApplication1
{
    internal class RedisCache : ICache, IDisposable
    {
        public RedisCache(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            _database = new Lazy<IDatabase>(() => CreateDatabase(connectionString));
        }


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

            return _database.Value.StringGet(key);
        }

        public void Set(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key");
            }

            if (value != null)
            {
                _database.Value.StringSet(key, value);
            }
            else
            {
                _database.Value.KeyDelete(key);
            }
        }

        public void Set(string key, string value, TimeSpan timemout)
        {
            throw new NotImplementedException();
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