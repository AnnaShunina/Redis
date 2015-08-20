using System;
using System.Runtime.Caching;
using NUnit.Framework;

namespace Redis
{
    [TestFixture]
    class MemoryChacheTest
    {
        private MemoryCacheImpl _cache;

        [SetUp]
        public void SetUp()
        {
            _cache = new MemoryCacheImpl("TestMemory");
        }
        [TearDown]
        public void TearDown()
        {
            _cache.Dispose();
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void ShouldThrownWhenNullOrEmptyContain(string key)
        {
            // Given

            // When
            TestDelegate tryMethod = () => _cache.Get(key);

            // Then
            Assert.Throws<ArgumentNullException>(tryMethod);
        }

        [Test]
        [TestCase("key1")]
        public void ShouldTrueWhenContain(string key)
        {
            // Given
            _cache.Set(key,"value1");
            // When
            bool exist = _cache.Contains(key);

            // Then
            Assert.IsTrue(exist);
        }

        [Test]
        [TestCase("key1")]
        public void ShouldFalseWhenNotContain(string key)
        {
            // Given
            _cache.Clear();
            // When
            bool exist = _cache.Contains(key);

            // Then
            Assert.IsFalse(exist);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void ShouldThrownWhenKeyIsEmptyOrNullGet(string key)
        {
            // Given

            // When
            TestDelegate result = () => _cache.Get(key);
            // Then
            Assert.Throws<ArgumentNullException>(result);
        }

        [Test]
        [TestCase("key1")]
        public void ShouldEmptyWhenValueIsEmptyAndKeyExistsGet(string key)
        {
            // Given

            // When
            _cache.Set("key1", "");
            var result = _cache.Get(key);
            // Then
            Assert.IsEmpty(result);
        }

        [Test]
        public void ShouldEqualGet()
        {
            //Given
            string key = "key1";
            string value = "example";
            //When
            _cache.Set(key, value);
            //Then
            string _value = _cache.Get(key);
            Assert.AreEqual(value, _value);
        }

        [Test]
        public void ShouldTrueWhenValueExistsTryGet()
        {
            //Given
            string key = "key1";
            string value = "example";
            //When
            _cache.Set(key, value);
            //Then
            string val;
            bool result = _cache.TryGet(key, out val);
            Assert.IsTrue(result);
        }

        [Test]
        [TestCase("key1", "value1")]
        public void ShouldRecieveWhenSet(string key, string value)
        {
            // Given

            // When
            _cache.Set(key, value);
            var actualValue = _cache.Get(key);

            // Then
            Assert.AreEqual(value, actualValue);
        }

        [Test]
        [TestCase("", "value1")]
        [TestCase(null, "value1")]
        [TestCase("  ", "value1")]
        public void ShouldThrownWhenSet(string key, string value)
        {
            // Given

            // When
            TestDelegate actualValue = () => _cache.Set(key, value);

            // Then
            Assert.Throws<ArgumentNullException>(actualValue);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void ShouldThrownWhenKeyIsNullOrEmptyRemove(string key)
        {
            // Given

            // When
            TestDelegate actualValue = () => _cache.Remove(key);

            // Then
            Assert.Throws<ArgumentNullException>(actualValue);
        }

        [Test]
        [TestCase("key1")]
        public void ShouldRemoveWhenRemove(string key)
        {
            // Given

            // When
            _cache.Remove(key);
            // Then
            string val = _cache.Get(key);
            Assert.IsNull(val);
        }

    }
}
