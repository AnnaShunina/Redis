using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Redis.Cache.Interface;

namespace Redis.Cache
{
    internal class TwoLayerCacheImpl : ICache, IDisposable
    {
        public TwoLayerCacheImpl(ICache cacheLayer1, ICache cacheLayer2, IMessageBus messageBus)
        {
            if (cacheLayer1 == null)
            {
                throw new ArgumentNullException("cacheLayer1");
            }

            if (cacheLayer2 == null)
            {
                throw new ArgumentNullException("cacheLayer2");
            }

            _cacheLayer1 = cacheLayer1;
            _cacheLayer2 = cacheLayer2;
            _messageBus = messageBus;
            _publisherId = Guid.NewGuid().ToString("N");
            _subscriptions = new ConcurrentDictionary<string, IDisposable>();
        }


        private readonly string _publisherId;
        private readonly ICache _cacheLayer1;
        private readonly ICache _cacheLayer2;
        private readonly IMessageBus _messageBus;
        private readonly ConcurrentDictionary<string, IDisposable> _subscriptions;


        public bool Contains(string key)
        {
            return _cacheLayer1.Contains(key) || _cacheLayer2.Contains(key);
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

            var exists = false;

            if (!_cacheLayer1.TryGet(key, out value))
            {
                if (!_cacheLayer2.TryGet(key, out value))
                {
                    value = null;
                }
                else
                {
                    exists = true;

                    _cacheLayer1.Set(key, value);

                    SubscribeOnKeyChanged(key);
                }
            }

            return exists;
        }

        public void Set(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            _cacheLayer1.Set(key, value);
            _cacheLayer2.Set(key, value);

            NotifyOnKeyChanged(key);
            SubscribeOnKeyChanged(key);
        }

        public void Remove(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key");
            }

            try
            {
                try
                {
                    _cacheLayer1.Remove(key);
                }
                finally
                {
                    _cacheLayer2.Remove(key);
                }
            }
            finally
            {
                NotifyOnKeyChanged(key);
                UnsubscribeOnKeyChanged(key);
            }
        }

        public void Clear()
        {
            try
            {
                try
                {
                    _cacheLayer1.Clear();
                }
                finally
                {
                    _cacheLayer2.Clear();
                }
            }
            finally
            {
                foreach (var subscription in _subscriptions.ToArray())
                {
                    NotifyOnKeyChanged(subscription.Key);
                    UnsubscribeOnKeyChanged(subscription.Key);
                }
            }
        }


        public void Dispose()
        {
            foreach (var subscription in _subscriptions.ToArray())
            {
                UnsubscribeOnKeyChanged(subscription.Key);
            }

        }


        private void NotifyOnKeyChanged(string key)
        {
            ExecuteAsync(() => _messageBus.Publish(key, _publisherId));
        }

        private void SubscribeOnKeyChanged(string key)
        {
            if (!_subscriptions.ContainsKey(key))
            {
                ExecuteAsync(() =>
                {
                    var subscription = _messageBus.Subscribe(key, (k, v) =>
                    {
                        if (v != _publisherId)
                        {
                            try
                            {
                                _cacheLayer1.Remove(k);
                            }
                            catch
                            {
                                // ignored
                            }
                        }
                    });

                    _subscriptions.TryAdd(key, subscription);
                });
            }
        }

        private void UnsubscribeOnKeyChanged(string key)
        {
            IDisposable subscription;

            if (_subscriptions.TryRemove(key, out subscription))
            {
                ExecuteAsync(subscription.Dispose);
            }
        }

        private static void ExecuteAsync(Action action)
        {
            Task.Run(() =>
            {
                try
                {
                    action();
                }
                catch
                {
                }
            });
        }
    }
}