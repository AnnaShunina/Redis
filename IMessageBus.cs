using System;

namespace ConsoleApplication1
{
    interface IMessageBus
    {
        void Publish(string key, string value);

        IDisposable Subscribe(string key, Action<string, string> handler);
    }
}
