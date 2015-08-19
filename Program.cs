using System;
using System.Configuration;
using System.Threading;

namespace Redis
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = ConfigurationManager.AppSettings["RedisConnectionString"];

            var cacheRedis = new RedisCacheImpl("redis", connectionString);
            var cacheMemory = new MemoryCacheImpl("memory");
            var subscriber = new RedisMessageBus(connectionString);

            var layerCacheImpl = new TwoLayerCacheImpl(cacheMemory, cacheRedis, subscriber);
            layerCacheImpl.Set("1","1");
            Console.WriteLine(layerCacheImpl.Get("1"));
            layerCacheImpl.Remove("1");
            Console.WriteLine(layerCacheImpl.Get("1"));
            layerCacheImpl.Set("2", "2");
            layerCacheImpl.Set("3", "3");
            layerCacheImpl.Set("4", "4");
            Console.WriteLine(layerCacheImpl.Contains("2"));
            layerCacheImpl.Clear();
            Console.WriteLine(layerCacheImpl.Get("2"));
            Console.ReadKey();
            layerCacheImpl.Dispose();
            //cacheMemory.Dispose();
            //cacheRedis.Dispose();
            //subscriber.Dispose();
        }

        private static void MyHandler(string key, string value)
        {
            Console.WriteLine("Subscriber: key={0}, value={1}", key, value);
        }
    }
}
