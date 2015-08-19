using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Redis
{
    internal class TwoLayerCacheImpl : ICache, IDisposable
    {
        public TwoLayerCacheImpl(ICache layer1, ICache layer2, IMessageBus messageBus)
        {
            if (layer1 == null)
            {
                throw new ArgumentNullException("layer1");
            }

            if (layer2 == null)
            {
                throw new ArgumentNullException("layer2");
            }

            _layer1 = layer1;
            _layer2 = layer2;
            _messageBus = messageBus;
            _subscriptions = new ConcurrentDictionary<string, IDisposable>();
        }


        private readonly ICache _layer1;
        private readonly ICache _layer2;
        private readonly IMessageBus _messageBus;
        private readonly ConcurrentDictionary<string, IDisposable> _subscriptions;


        public bool Contains(string key)
        {
            return _layer1.Contains(key) || _layer2.Contains(key);
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
            
            if (_layer1.TryGet(key, out value))
            {
                if (_layer2.TryGet(key, out value))
                {
                    value = null;
                }
            }

            // Sub
            bool exist = false;
            foreach (var subscription in _subscriptions.ToArray())
            {
                if (subscription.Key == key)
                { 
                    exist = true;
                    break;
                }
            }
            if (exist == false)
            {
                Subscribe(new KeyValuePair<string, IDisposable>(key, _messageBus.Subscribe(key, MyHandler)));
            }
            return (value == null);
        }

        // todo: Накапливать задачи и отправлять на исполнение
        public void Set(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (_layer2.Get(key) == value && _layer1.Get(key) == value) return;
            if(_layer2.Contains(key)) Publish(key, "Change a value of key: " + key + " to " + value);

            if(_layer1.Get(key) != value) _layer1.Set(key, value);
            if(_layer2.Get(key) != value) _layer2.Set(key, value);

            // Sub
            bool exist = false;
            foreach (var subscription in _subscriptions.ToArray())
            {
                if (subscription.Key == key)
                {
                    exist = true;
                    break;
                }
            }
            if (exist == false)
            {
                Subscribe(new KeyValuePair<string, IDisposable>(key, _messageBus.Subscribe(key, MyHandler)));
            }
        }

        private static void MyHandler(string key, string value)
        {
            Console.WriteLine("Subscriber: key={0}, value={1}", key, value);
        }

        public void Remove(string key)
        {

            // Unsub
            foreach (var subscription in _subscriptions.ToArray())
            {
                if (subscription.Key == key)
                {
                    Publish(key, "Remove key: " + subscription.Key);
                    Unsubscribe(subscription.Key);
                    break;
                }
            }
            // ??
            try
            {
                _layer1.Remove(key);
            }
            finally
            {
                _layer2.Remove(key);
            }
            
        }

        public void Clear()
        {

            // Unsub
            foreach (var subscription in _subscriptions.ToArray())
            {
                Publish(subscription.Key, "Clear all: " + subscription.Key);
                Unsubscribe(subscription.Key);
            }
            // ??
            try
            {
                _layer1.Clear();
            }
            finally
            {
                _layer2.Clear();
            }
            
        }

        public void Dispose()
        {

            // Unsub
            foreach (var subscription in _subscriptions.ToArray())
            {
                Publish(subscription.Key,"Dispose: " + subscription.Key);
                Unsubscribe(subscription.Key);
            }
            
            var dispose1 = _layer1 as IDisposable;

            if (dispose1 != null)
            {
                dispose1.Dispose();
            }

            var dispose2 = _layer2 as IDisposable;

            if (dispose2 != null)
            {
                dispose2.Dispose();
            }
        }

        public void Publish(string key, string value)
        {
            _messageBus.Publish(key, value);
        }

        private void Subscribe(KeyValuePair<string, IDisposable> subscriptionPair)
        {
            _subscriptions.TryAdd(subscriptionPair.Key, subscriptionPair.Value);
        }

        private void Unsubscribe(string key)
        {
            IDisposable deletedSubscription;

            if (_subscriptions.TryRemove(key, out deletedSubscription))
            {
                deletedSubscription.Dispose();
            }
        }
    }
}