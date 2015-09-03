﻿using System;
using System.Configuration;
using System.Diagnostics;
using NUnit.Framework;

namespace Redis.Cache.Tests.Perfomance
{
    [TestFixture]
    internal sealed class PerfomanceTwoLayerRealCacheTest
    {
        private readonly TwoLayerCacheImpl _testCache;


        public PerfomanceTwoLayerRealCacheTest()
        {
            var connectionString = ConfigurationManager.AppSettings["RedisConnectionString"];
            MemoryCacheImpl cacheLayer1 = new MemoryCacheImpl("PerfomanceTwoLayerRealCacheTest");
            RedisCacheImpl cacheLayer2 = new RedisCacheImpl("PerfomanceTwoLayerRealCacheTest", connectionString);
            RedisMessageBus messageBus = new RedisMessageBus(connectionString);
            _testCache = new TwoLayerCacheImpl(cacheLayer1, cacheLayer2, messageBus);
        }

        [Test]
        [TestCase(100000)]
        [Ignore("Manual")]
        public void TestGet(int iterations)
        {
            //Given
            _testCache.Set("key1", "value1");
            //When
            Stopwatch stopwatch = new Stopwatch();
            for (var i = 0; i < iterations; i++)
            {
                stopwatch.Start();
                _testCache.Get("key1");
                stopwatch.Stop();
            }
            //Then
            double avg = stopwatch.Elapsed.TotalMilliseconds / iterations;
            Console.WriteLine("Iterations: {0}", iterations);
            Console.WriteLine("Avg time: {0:F}", avg);
            Console.WriteLine("Per a second: {0}", 1000 / avg);
        }

        [Test]
        [TestCase(100000)]
        [Ignore("Manual")]
        public void TestSet(int iterations)
        {
            //Given
            Stopwatch stopwatch = new Stopwatch();
            //When
            for (var i = 0; i < iterations; i++)
            {
                stopwatch.Start();
                _testCache.Set("key1", "value1");
                stopwatch.Stop();
                _testCache.Remove("key1");
            }
            //Then
            double avg = stopwatch.Elapsed.TotalMilliseconds / iterations;
            Console.WriteLine("Iterations: {0}", iterations);
            Console.WriteLine("Avg time: {0:F}", avg);
            Console.WriteLine("Per a second: {0}", 1000 / avg);
        }
    }
}