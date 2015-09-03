using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Redis.Cache.Interface;

namespace Redis.Fake
{
    internal sealed class FakeMessageBus : IMessageBus, IDisposable
    {
        private readonly Dictionary<string, List<FakeSubscriber>> _subscribers
            = new Dictionary<string, List<FakeSubscriber>>();


        public Task Publish(string key, string value)
        {
            return Task.Run(() =>
            {
                lock (this)
                {
                    List<FakeSubscriber> keySubscribers;

                    if (_subscribers.TryGetValue(key, out keySubscribers))
                    {
                        foreach (var subscriber in keySubscribers)
                        {
                            subscriber.Handle(key, value);
                        }
                    }
                }
            });
        }

        public IDisposable Subscribe(string key, Action<string, string> handler)
        {
            FakeSubscriber subscriber;

            lock (this)
            {
                List<FakeSubscriber> keySubscribers;

                if (!_subscribers.TryGetValue(key, out keySubscribers))
                {
                    keySubscribers = new List<FakeSubscriber>();
                    _subscribers.Add(key, keySubscribers);
                }

                subscriber = new FakeSubscriber(handler, s =>
                {
                    lock (this)
                    {
                        keySubscribers.Remove(s);
                    }
                });

                keySubscribers.Add(subscriber);
            }

            return subscriber;
        }

        public void Dispose()
        {
            lock (this)
            {
                _subscribers.Clear();
            }
        }


        internal sealed class FakeSubscriber : IDisposable
        {
            public FakeSubscriber(Action<string, string> handler, Action<FakeSubscriber> unsubscribe)
            {
                _handler = handler;
                _unsubscribe = unsubscribe;
            }

            private readonly Action<string, string> _handler;
            private readonly Action<FakeSubscriber> _unsubscribe;

            public void Handle(string key, string value)
            {
                _handler(key, value);
            }

            public void Dispose()
            {
                _unsubscribe(this);
            }
        }
    }
}