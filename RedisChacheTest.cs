using System;
using System.Configuration;

using NUnit.Framework;

namespace ConsoleApplication1
{
    [TestFixture]
    class RedisChacheTest
    {
        [SetUp]
        public void SetUp()
        {
            var connectionString = ConfigurationManager.AppSettings.Get("RedisConnectionString");

            _cache = new RedisCache(connectionString);
        }

        [TearDown]
        public void TearDown()
        {
            _cache.Dispose();
        }


        private RedisCache _cache;

        [Test]
        public void ShouldThrowExceptionWhenKeyIsEmpty()
        {
            // Given
            string key = "";

            // When
            TestDelegate tryMethod = () => _cache.Get(key);

            // Then
            Assert.That(tryMethod,Is.Empty);
        }

        [Test]
        public void ShouldThrowExceptionWhenKeyIsNull()
        {
            // Given
            string key = null;

            // When
            TestDelegate tryMethod = () => _cache.Get(key);

            // Then
            Assert.Throws<ArgumentNullException>(tryMethod);
        }

        [Test]
        public void ShouldStoreValue()
        {
            // Given
            var key = Guid.NewGuid().ToString();
            var value = Guid.NewGuid().ToString();

            // When
            _cache.Set(key, value);
            var actualValue = _cache.Get(key);

            // Then
            Assert.AreEqual(value, actualValue);
        }
    }
}