using System;
using StackExchange.Redis;

namespace Redis
{
    internal sealed class RedisMessageBus : IMessageBus
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

        private static ISubscriber CreateSubscriber(string connectionString)
        {
            var connection = ConnectionMultiplexer.Connect(connectionString);
            var subscriber = connection.GetSubscriber();
            return subscriber;
        }


        public void Publish(string key, string value)
        {
            _subscriber.Value.Publish(key, value);
        }

        public IDisposable Subscribe(string key, Action<string, string> handler)
        {
            var subscription = new RedisChannel(key, RedisChannel.PatternMode.Literal);

            var subscriber = new RedisSubscriber(() => _subscriber.Value.Unsubscribe(subscription));

            _subscriber.Value.Subscribe(subscription, (k, v) => handler(k, v));

            return subscriber;
        }


        public void Dispose()
        {
            if (_subscriber.IsValueCreated)
            {
                var connection = _subscriber.Value.Multiplexer;
                connection.Close();
                connection.Dispose();
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
