using System;
using System.Configuration;
using NUnit.Framework;

namespace Redis.Cache.RedisCache
{
    [TestFixture]
    class RedisCacheTest
    {
        [SetUp]
        public void SetUp()
        {
            var connectionString = ConfigurationManager.AppSettings.Get("RedisConnectionString");

            _cache = new RedisCacheImpl("TestNamespace", connectionString);
        }

        [TearDown]
        public void TearDown()
        {
            _cache.Dispose();
        }


        private RedisCacheImpl _cache;

        [Test]
        [TestCase("key1","value1")]
        public void ShouldReturnsTrueWhenKeyIsExistContains(string key, string value)
        {
            // Given
            _cache.Set(key,value);
            // When
            bool result = _cache.Contains(key);
            // Then
            Assert.True(result);
        }

        [Test]
        [TestCase("key1", "value1")]
        public void ShouldReturnsFalseWhenKeyIsNotExistContains(string key, string value)
        {
            // Given
            _cache.Clear();
            // When
            bool result = _cache.Contains(key);
            // Then
            Assert.False(result);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void ShouldThrowExceptionWhenKeyIsEmptyOrNullContains(string key)
        {
            // Given

            // When
            TestDelegate result = () => _cache.Contains(key);
            // Then
            Assert.Throws<ArgumentNullException>(result);
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
            _cache.Set(key,value);
            //Then
            string value2 = _cache.Get(key);
            Assert.AreEqual(value, value2);
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
        [TestCase("key1","value1")]
        public void ShouldRecieveWhenSet(string key,string value)
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
            TestDelegate actualValue = ()=> _cache.Set(key,value);

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

        [Test]
        public void ShouldCorrectClear()
        {
            //Given
            //When
            _cache.Set("key1", "value1");
            _cache.Set("key2", "value2");
            _cache.Set("key3", "value3");
            _cache.Clear();
            //Then
            var result = _cache.Contains("key1") || _cache.Contains("key2") || _cache.Contains("key3");
            Assert.IsFalse(result);
        }

    }
}