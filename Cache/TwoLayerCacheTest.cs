using System.Threading;
using NUnit.Framework;
using Redis.Fake;

namespace Redis.Cache
{
    [TestFixture]
    class TwoLayerCacheTest
    {
        [SetUp]
        public void SetUp()
        {
            _layer1 = new FakeCacheImpl();
            _layer2 = new FakeCacheImpl();
            _messageBus = new FakeMessageBus();
            _twoLayerCache = new TwoLayerCacheImpl(_layer1, _layer2, _messageBus);
        }

        [TearDown]
        public void TearDown()
        {
            _twoLayerCache.Dispose();
        }

        private FakeCacheImpl _layer1;
        private FakeCacheImpl _layer2;
        private FakeMessageBus _messageBus;
        private TwoLayerCacheImpl _twoLayerCache;

        [Test]
        [TestCase("")]
        [TestCase("1")]
        [TestCase("2")]
        [TestCase("null")]
        public void KeyIsContains(string key)
        {
            //Given
            
            //When
            var exist = _twoLayerCache.Contains(key);

            //Then
            Assert.IsFalse(exist);
        }

        [Test]
        public void CorrectTryGetInLayer1()
        {
            //Given
            const string key = "key1";
            const string val = "value1";
            //When
            _layer1.Data[key] = val;
            string value;
            _twoLayerCache.TryGet(key, out value);
            //Then
            Assert.AreEqual(value,val);
        }

        [Test]
        public void CorrectTryGetInLayer2()
        {
            //Given
            const string key = "key1";
            const string val = "value1";
            //When
            _layer2.Data[key] = val;
            string value;
            var result = _twoLayerCache.TryGet(key, out value);
            //Then
            Assert.IsTrue(result);
        }

        [Test]
        public void CorrectGetInLayer1()
        {
            //Given
            const string key = "key1";
            const string val = "value1";
            //When
            _layer1.Data[key] = val;
            string value = _twoLayerCache.Get(key);
            var val2 = _layer1.Get(key);
            //Then
            Assert.AreEqual(value, val);
            Assert.AreEqual(value, val2);
        }

        [Test]
        public void CorrectGetInLayer2()
        {
            //Given
            const string key = "key1";
            const string val = "value1";
            //When
            _layer2.Data[key] = val;
            string value = _twoLayerCache.Get(key);
            var val2 = _layer2.Get(key);
            //Then
            Assert.AreEqual(value, val);
            Assert.AreEqual(value, val2);
        }

        [Test]
        public void CorrectSet()
        {
            //Given
            const string key = "key1";
            const string value = "value1";
            //When
            _twoLayerCache.Set(key,value);
            //Then
            Assert.AreEqual(value,_layer1.Data["key1"]);
            Assert.AreEqual(value, _layer2.Data["key1"]);
        }

        [Test]
        public void CorrectRemove()
        {
            //Given
            const string key = "key1";
            const string value = "value1";
            //When
            _twoLayerCache.Set(key, value);
            _twoLayerCache.Remove(key);
            //Then
            string val;
            bool exist = _twoLayerCache.TryGet(key, out val);
            Assert.IsFalse(exist);
        }

        [Test]
        public void CorrectClear()
        {
            //Given
            const string key1 = "key1";
            const string value1 = "value1";
            const string key2 = "key2";
            const string value2 = "value2";
            const string key3 = "key3";
            const string value3 = "value3";
            //When
            _twoLayerCache.Set(key1, value1);
            _twoLayerCache.Set(key2, value2);
            _twoLayerCache.Set(key3, value3);
            _twoLayerCache.Clear();
            //Then
            Assert.IsFalse(_twoLayerCache.Contains(key1) || _twoLayerCache.Contains(key2) || _twoLayerCache.Contains(key3)); 
        }

        [Test]
        public void ShouldNotifyWhenSet()
        {
            //Given
            var published = false;
            _messageBus.Subscribe("key1", (k, v) => published = true);
            //When
            _twoLayerCache.Set("key1", "value1");
            //Then
            Assert.IsTrue(published);
        }

        [Test]
        public void ShouldInformWhenRemove()
        {
            //Given
            var published = false;
            //When
            _twoLayerCache.Set("key1","value1");
            _messageBus.Subscribe("key1", (k, v) => published = true);
            _twoLayerCache.Remove("key1");
            //Then
            Assert.IsTrue(published);
        }

        [Test]
        public void ShouldInformWhenClear()
        {
            //Given
            var published = false;
            //When
            _twoLayerCache.Set("key1", "value1");
            _twoLayerCache.Set("key2", "value2");
            _twoLayerCache.Set("key3", "value3");
            _messageBus.Subscribe("key1", (k, v) => published = true);
            _twoLayerCache.Clear();
            //Then
            Assert.IsTrue(published);
        }

        [Test]
        public void ShouldNotifyOneTime()
        {
            //Given
            int[] a = {0};
            //When
            _messageBus.Subscribe("key1", (k, v) => Interlocked.Increment(ref a[0]));
            _twoLayerCache.Set("key1", "value1");
            _twoLayerCache.Set("key1", "value2");
            Thread.Sleep(1000);
            //Then
            Assert.AreEqual(1, Thread.VolatileRead(ref a[0]));
        }
    }
}
