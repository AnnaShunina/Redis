using System;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Redis.Cache.Interface;
using StackExchange.Redis;

namespace Redis.Cache
{
    internal sealed class RedisMessageBus : IMessageBus, IDisposable
    {
        public RedisMessageBus(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            _subscriber = new Lazy<ISubscriber>(() => CreateSubscriber(connectionString));
        }


        private readonly Lazy<ISubscriber> _subscriber;
        private const int MaxPublishAttempts = 5;
        private readonly static TimeSpan MaxPublishAttempDelay = new TimeSpan(0, 0, 0, 10);
        private static ConnectionMultiplexer _connection;
        private static ISubscriber CreateSubscriber(string connectionString)
        {
            _connection = ConnectionMultiplexer.Connect(connectionString);
            var subscriber = _connection.GetSubscriber();
            return subscriber;
        }

        public void TryPublish(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key");
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException("value");
            }
            Task.Run(() =>
            {
                for (int i = 0; i < MaxPublishAttempts; i++)
                {
                    try
                    {
                        //_subscriber.Value.Publish(key, value, CommandFlags.NoRedirect);
                        _subscriber.Value.Publish(key, value);
                        break;
                    }
                    catch
                    {
                        if (i + 1 == MaxPublishAttempts)
                        {
                            throw new TimeoutException();
                        }
                    }
                    Thread.Sleep(MaxPublishAttempDelay);
                }
            }).Wait();
        }

        public Task Publish(string key, string value)
        {

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key");
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException("value");
            }

            return Task.Run(() => _subscriber.Value.Publish(key, value));
        }

        public IDisposable Subscribe(string key, Action<string, string> handler)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key");
            }
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            var subscription = new RedisChannel(key, RedisChannel.PatternMode.Literal);

            var subscriber = new RedisSubscriber(() => _subscriber.Value.Unsubscribe(subscription));

            _subscriber.Value.Subscribe(subscription, (k, v) => handler(k, v));

            return subscriber;
        }


        public void Dispose()
        {
            if (_subscriber.IsValueCreated)
            {
                //var connection = _subscriber.Value.Multiplexer;
                //connection.Close();
                //connection.Dispose();
                _connection.Close();
                _connection.Dispose();
            }
        }


        internal sealed class RedisSubscriber : IDisposable
        {
            private readonly Action _unsubscribe;
            public RedisSubscriber(Action unsubscribe)
            {
                _unsubscribe = unsubscribe;
            }

            public void Dispose()
            {
                _unsubscribe();
            }
        }
    }
}
