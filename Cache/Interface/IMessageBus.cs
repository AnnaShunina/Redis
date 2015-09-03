using System;
using System.Threading.Tasks;

namespace Redis.Cache.Interface
{
    interface IMessageBus
    {
        Task Publish(string key, string value);

        IDisposable Subscribe(string key, Action<string, string> handler);
    }
}
