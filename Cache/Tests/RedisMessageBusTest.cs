using System;
using System.Configuration;
using System.Threading;
using NUnit.Framework;

namespace Redis.Cache.Tests
{
    [TestFixture]
    class RedisMessageBusTest
    {
        [SetUp]
        public void SetUp()
        {
            var connectionString = ConfigurationManager.AppSettings.Get("RedisConnectionString");
            _messageBus = new RedisMessageBus(connectionString);
        }

        [TearDown]
        public void TearDown()
        {
            _messageBus.Dispose();
        }

        private RedisMessageBus _messageBus;

        [Test]
        [TestCase(null,null)]
        [TestCase(null, "")]
        [TestCase("", null)]
        [TestCase("", "")]
        public void ShouldThrownWhenNullOrEmptyPublish(string key, string value)
        {
            //Given
            //When
            TestDelegate resultDelegate = () => _messageBus.Publish(key, value);
            //Then
            Assert.Throws<ArgumentNullException>(resultDelegate);
        }

        [Test]
        public void ShouldPublish()
        {
            int[] a = {0};
            var sub = _messageBus.Subscribe("test", delegate { Interlocked.Increment(ref a[0]); });
            _messageBus.Publish("test","test");
            Thread.Sleep(1000);
            Assert.AreEqual(1, Thread.VolatileRead(ref a[0]));
            sub.Dispose();
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void ShouldThrownWhenKeyNullOrEmptySubscribe(String key)
        {
            //Given
            //When
            TestDelegate testDelegate = ()=> _messageBus.Subscribe(key, delegate { });
            //Then
            Assert.Throws<ArgumentNullException>(testDelegate);
        }

        [Test]
        public void ShouldInvokeAllSubscribers()
        {
            // Given
            var subscriberKey1 = new CountdownEvent(2);
            var subscriberKey2 = new CountdownEvent(1);
            var subscriber1 = _messageBus.Subscribe("key1", (k, v) => subscriberKey1.Signal());
            var subscriber2 = _messageBus.Subscribe("key1", (k, v) => subscriberKey1.Signal());
            var subscriber3 = _messageBus.Subscribe("key2", (k, v) => subscriberKey2.Signal());

            // When
            _messageBus.Publish("key1", "value1");
            _messageBus.Publish("key2", "value2");

            // Then
            Assert.IsTrue(subscriberKey1.Wait(5000));
            Assert.IsTrue(subscriberKey2.Wait(5000));

            subscriber1.Dispose();
            subscriber2.Dispose();
            subscriber3.Dispose();
        }

        [Test]
        public void ShouldTrueWhenPublishWork()
        {
            //Given
            var subscriberKey = new CountdownEvent(1);
            var subscriber = _messageBus.Subscribe("key1", (k, v) => subscriberKey.Signal());
            //When

            _messageBus.TryPublish("key1", "value1");
            
            //Then
            Assert.IsTrue(subscriberKey.Wait(5000));
            subscriber.Dispose();
        }
    }
}
