using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemoryCache;
using StackExchange.Redis;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var cacheRedis = new RedisCache("RedisConnectionString");
            cacheRedis.Subscribe("line");
            cacheRedis.Dispose();
            Console.ReadKey();
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
