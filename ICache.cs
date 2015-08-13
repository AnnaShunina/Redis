using System;

namespace ConsoleApplication1
{
    interface ICache
    {
        string Get(string key);
        void Set(string key, string value);
        void Set(string key, string value, TimeSpan timemout);
    }
}