using System;
using System.Collections.Generic;

namespace Redis
{
    internal sealed class FakeMessageBus : IMessageBus, IDisposable
    {
        public readonly Dictionary<string, FakeSubscriber> _subscribers
            = new Dictionary<string, FakeSubscriber>();


        public void Publish(string key, string value)
        {
            FakeSubscriber subscriber;

            if (_subscribers.TryGetValue(key, out subscriber))
            {
                subscriber.Handle(key, value);
            }
        }

        public IDisposable Subscribe(string key, Action<string, string> handler)
        {
            var subscriber = new FakeSubscriber(handler, () => _subscribers.Remove(key));

            _subscribers[key] = subscriber;

            return subscriber;
        }

        public void Dispose()
        {
            _subscribers.Clear();
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