namespace ConsoleApplication1
{
    interface ICache
    {
        bool Contains(string key);

        string Get(string key);

        bool TryGet(string key, out string value);

        void Set(string key, string value);

        void Remove(string key);

        void Clear();
    }
}