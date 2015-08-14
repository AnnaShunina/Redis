using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemoryCache;
using NUnit.Framework;

namespace ConsoleApplication1
{
    [TestFixture]
    class MemoryChacheTest
    {
        private MemoryCache _cache;

        [SetUp]
        public void SetUp()
        {
            _cache = new MemoryCache();
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
            _cache.Set(key, value);
            var actualValue = _cache.Get(key);

            // Then
            Assert.AreEqual(value, actualValue);
        }

    }
}
