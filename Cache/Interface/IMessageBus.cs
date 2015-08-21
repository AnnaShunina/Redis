using System;

namespace Redis
{
    interface IMessageBus
    {
        void Publish(string key, string value);

        IDisposable Subscribe(string key, Action<string, string> handler);
    }
}
