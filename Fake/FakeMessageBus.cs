using System;
using System.Collections.Generic;
using Redis.Cache.Interface;

namespace Redis.Fake
{
    internal sealed class FakeMessageBus : IMessageBus, IDisposable
    {
        public readonly Dictionary<string, FakeSubscriber> Subscribers
            = new Dictionary<string, FakeSubscriber>();


        public void Publish(string key, string value)
        {
            FakeSubscriber subscriber;

            if (Subscribers.TryGetValue(key, out subscriber))
            {
                subscriber.Handle(key, value);
            }
        }

        public IDisposable Subscribe(string key, Action<string, string> handler)
        {
            var subscriber = new FakeSubscriber(handler, () => Subscribers.Remove(key));

            Subscribers[key] = subscriber;

            return subscriber;
        }

        public void Dispose()
        {
            Subscribers.Clear();
        }


        internal sealed class FakeSubscriber : IDisposable
        {
            public FakeSubscriber(Action<string, string> handler, Action unsubscribe)
            {
                _handler = handler;
                _unsubscribe = unsubscribe;
            }

            private readonly Action<string, string> _handler;
            private readonly Action _unsubscribe;

            public void Handle(string key, string value)
            {
                _handler(key, value);
            }

            public void Dispose()
            {
                _unsubscribe();
            }
        }
    }
}