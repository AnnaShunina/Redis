using System;
using System.Threading;
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
                    _database.Value.StringSet(key, value, timemout);
                }
                else
                {
                    _database.Value.KeyDelete(key);
                }
            }
        }

        public void Subscribe(string sub)
        {
            ISubscriber _sub;
            _sub.Subscribe(sub, MyHandler);
        }

        private void MyHandler(RedisChannel arg1, RedisValue arg2)
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