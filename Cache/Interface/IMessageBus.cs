using System;

namespace Redis.Cache.Interface
{
    interface IMessageBus
    {
        void Publish(string key, string value);

        IDisposable Subscribe(string key, Action<string, string> handler);
    }
}
