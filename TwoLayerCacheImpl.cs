using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ConsoleApplication1
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
            return _layer1.Contains(key)
                   || _layer2.Contains(key);
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

            if (!_layer1.TryGet(key, out value))
            {
                if (!_layer2.TryGet(key, out value))
                {
                    value = null;
                }
            }

            return (value == null);
        }

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

            _layer1.Set(key, value);
            _layer2.Set(key, value);
        }

        public void Remove(string key)
        {
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
    }
}