using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MemoryCache;
using StackExchange.Redis;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = ConfigurationManager.AppSettings["RedisConnectionString"];

            var cacheRedis = new RedisCache(connectionString);
            // something code
            cacheRedis.Dispose();

            var subscriber = new RedisMessageBus(connectionString);
            // something code

            subscriber.Subscribe("key1", MyHandler);
            subscriber.Subscribe("key2", MyHandler);
            subscriber.Subscribe("key3", MyHandler);

            var p = Process.GetCurrentProcess().Id;

            for (int i = 0; i < 100; i++)
            {
                subscriber.Publish("key1", string.Format("vale1 pid={0}, index={1}", p, i));
                subscriber.Publish("key2", string.Format("vale2 pid={0}, index={1}", p, i));
                subscriber.Publish("key3", string.Format("vale3 pid={0}, index={1}", p, i));
                subscriber.Publish("key4", string.Format("vale4 pid={0}, index={1}", p, i));

                Thread.Sleep(1000);
            }

            subscriber.Dispose();

            Console.ReadKey();
        }

        private static void MyHandler(string key, string value)
        {
            Console.WriteLine("Subscriber: key={0}, value={1}", key, value);
        }

        //static void Create(IDatabase db)
        //{
        //    Console.WriteLine("Input key and value");
        //    string key, value;
        //    key = Console.ReadLine();
        //    value = Console.ReadLine();
        //    if (db.StringGet(key) == true)
        //    {
        //        Console.WriteLine("Key is exist");
        //    }
        //    else
        //    {
        //        db.StringSet(key, value);
        //        Console.WriteLine("Create " + key + ": " + db.StringGet(key));
        //    }
        //}

        //static void Update(IDatabase db)
        //{
        //    Console.WriteLine("Input key and value");
        //    string key, value;
        //    key = Console.ReadLine();
        //    if (db.StringGet(key)==true)
        //    {
        //        value = Console.ReadLine();
        //        db.StringSet(key, value);
        //        Console.WriteLine("Updated");
        //    }
        //    else
        //    {
        //        Console.WriteLine("Doesn't exist key");
        //    }
        //}

        //static void Show(IDatabase db)
        //{
        //    Console.WriteLine("Output, input a key");
        //    string key;
        //    key = Console.ReadLine();
        //    var a = db.StringGet(key);
        //    if (!a.IsNull)
        //    {
        //        Console.WriteLine(db.StringGet(key));
        //    }
        //    else
        //    {
        //        Console.WriteLine("Doesn't exist key");
        //    }
        //}

        //static void Delete(IDatabase db)
        //{
        //    Console.WriteLine("Input a key");
        //    string key;
        //    key = Console.ReadLine();
        //    var a = db.StringGet(key);
        //    if (!a.IsNull)
        //    {
        //        db.KeyDelete(key);
        //        Console.WriteLine("Key have removed successful");
        //    }
        //    else
        //    {
        //        Console.WriteLine("Doesn't exist key");
        //    }
        //}
    }
}
