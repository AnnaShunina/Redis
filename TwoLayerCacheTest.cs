using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Diagnostics;
using NUnit.Framework;

namespace Redis
{
    [TestFixture]
    class TwoLayerCacheTest
    {
        [SetUp]
        public void SetUp()
        {
            var connectionString = ConfigurationManager.AppSettings["RedisConnectionString"];
            _layerRedisCache = new RedisCacheImpl("redis",connectionString);
            _layerMemoryCacheCache = new MemoryCacheImpl("memory");
            _messageBus = new RedisMessageBus(connectionString);
            _layer = new TwoLayerCacheImpl(_layerRedisCache, _layerMemoryCacheCache, _messageBus);
        }

        [TearDown]
        public void TearDown()
        {
            _layer.Dispose();
        }

        private TwoLayerCacheImpl _layer;
        private ICache _layerRedisCache;
        private ICache _layerMemoryCacheCache;
        private IMessageBus _messageBus;
        private readonly ConcurrentDictionary<string, IDisposable> _subscriptions;

        [Test]
        [TestCase("")]
        [TestCase("1")]
        [TestCase("2")]
        [TestCase("null")]
        public void KeyIsContains(string key)
        {
            //Given
            _layer.Clear();
            //When
            bool exist = _layer.Contains(key);
            //Then
            Assert.IsFalse(exist);
        }

        [Test]
        public void CorrectTryGet()
        {
            //Given

            //When

            //Then
        }

        [Test]
        public void CorrectGet()
        {
            //Given

            //When

            //Then
        }

        [Test]
        public void CorrectSet()
        {
            //Given

            //When

            //Then
        }

        [Test]
        public void CorrectRemove()
        {
            //Given

            //When

            //Then
        }

        [Test]
        public void CorrectClear()
        {
            //Given

            //When

            //Then
        }

        [Test]
        public void CorrectPublish()
        {
            //Given

            //When

            //Then
        }

        [Test]
        public void CorrectSubscribe()
        {
            //Given

            //When

            //Then
        }

        [Test]
        public void CorrectUnsubscribe()
        {
            //Given

            //When

            //Then
        }

        [Test]
        public void SubscribeWhenGetting()
        {
            //Given

            //When

            //Then
        }

        [Test]
        public void SubscribeWhenSettingAndPublishIfChange()
        {
            //Given

            //When

            //Then
        }

        [Test]
        public void RemoveAndPublishAndUnsubscribeAll()
        {
            //Given

            //When

            //Then
        }

        [Test]
        public void ClearAllAndUnsubscribeAll()
        {
            //Given

            //When

            //Then
        }
        
    }
}
