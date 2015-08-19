using System;
using System.Runtime.Caching;
using NUnit.Framework;

namespace Redis
{
    [TestFixture]
    class MemoryChacheTest
    {
        private MemoryCache _cache;

        [SetUp]
        public void SetUp()
        {
            _cache = new MemoryCache("example");
        }
        [TearDown]
        public void TearDown()
        {
            _cache.Dispose();
        }

        [Test]
        public void ShouldThrowExceptionWhenKeyIsEmpty()
        {
            // Given
            string key = "";

            // When
            TestDelegate tryMethod = () => _cache.Get(key);

            // Then
            Assert.IsNullOrEmpty(key);
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
            _cache.Set(key, value, new CacheItemPolicy());
            var actualValue = _cache.Get(key);

            // Then
            Assert.AreEqual(value, actualValue);
        }

    }
}
