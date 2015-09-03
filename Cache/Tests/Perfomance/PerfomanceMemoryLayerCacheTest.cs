using System;
using System.Diagnostics;
using NUnit.Framework;

namespace Redis.Cache.Tests.Perfomance
{
    [TestFixture]
    internal sealed class PerfomanceMemoryLayerCacheTest
    {
        private MemoryCacheImpl _testCache;

        public PerfomanceMemoryLayerCacheTest()
        {
            _testCache = new MemoryCacheImpl("PerfomanceRedisLayerCacheTest");
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
