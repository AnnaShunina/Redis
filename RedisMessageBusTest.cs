using System;
using System.Configuration;
using System.Threading;
using NUnit.Framework;

namespace Redis
{
    [TestFixture]
    class RedisMessageBusTest
    {
        [SetUp]
        public void SetUp()
        {
            var connectionString = ConfigurationManager.AppSettings["RedisConnectionString"];
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
            int a = 0;
            var sub = _messageBus.Subscribe("test", delegate { Interlocked.Increment(ref a); });
            _messageBus.Publish("test","verify");
            Assert.AreEqual(1, Thread.VolatileRead(ref a));
            sub.Dispose();
        }
        //public void ShouldThrownWhenKeyNullOrEmptySubscribe()
        //{
        //    //Given
        //    //When
        //    //Then
        //}
    }
}
