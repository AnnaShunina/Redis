using System;
using NUnit.Framework;

namespace Redis.Cache.Tests.GC
{
    [TestFixture]
    internal sealed class GCMemoryLayerCacheTest
    {
        private MemoryCacheImpl _testCache;
        public void SetUp()
        {
            _testCache = new MemoryCacheImpl("MemoryLayerCacheTest");
        }

        public void TearDown()
        {
            _testCache.Dispose();
            _testCache = null;
        }


        [Test]
        [TestCase(100)]
        [TestCase(1000)]
        [TestCase(10000)]
        [TestCase(100000)]
        [TestCase(200000)]
        [Ignore("Manual")]
        public void Test(int iterations)
        {
            //Given
            double startSize = System.GC.GetTotalMemory(true);

            SetUp();
            //When
            for (var i = 0; i < iterations; i++)
            {
                _testCache.Set("key1", "value1");
                _testCache.Get("key1");
            }

            TearDown();

            double stopSize = System.GC.GetTotalMemory(true);

            var memoryLeak = (stopSize - startSize);
            //Then
            Console.WriteLine("Iterations: {0}", iterations);
            Console.WriteLine("Memory Leak: {0:P}", memoryLeak / startSize);
            Console.WriteLine("Memory Leak: {0:N2} Kb", memoryLeak / 1024);
        }
    }
}