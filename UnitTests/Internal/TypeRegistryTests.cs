using NGineer.Internal;
using NUnit.Framework;

namespace NGineer.UnitTests.Internal
{
    [TestFixture]
    public class TypeRegistryTests
    {
        private TypeRegistry<StoreClass> _store;

        [SetUp]
        public void SetUp()
        {
            _store = new TypeRegistry<StoreClass>();
        }

        [Test]
        public void GetForType_UndefinedType()
        {
            Assert.IsNull(_store.GetForType<TestClass>());
        }

        [Test]
        public void SetForType_SetValueIsRetrieved()
        {
            var storeValue = new StoreClass();
            _store.SetForType<TestClass>(storeValue);
            Assert.AreSame(storeValue, _store.GetForType<TestClass>());
        }

        [Test]
        public void SetForType_SetValueOverridesExisting()
        {
            var storeValue = new StoreClass();
            _store.SetForType<TestClass>(storeValue);
            var newStoreValue = new StoreClass();
            _store.SetForType<TestClass>(newStoreValue);
            Assert.AreSame(newStoreValue, _store.GetForType<TestClass>());
        }

        [Test]
        public void GetForType_DefinedForInterface_FetchedOnClass_ReturnsStoredValue()
        {
            var storeValue = new StoreClass();
            _store.SetForType<ITestClass>(storeValue);
            Assert.AreSame(storeValue, _store.GetForType<TestClass>());
        }

        [Test]
        public void GetForType_DefinedForClass_FetchedOnInterface_ReturnsNull()
        {
            _store.SetForType<TestClass>(new StoreClass());
            Assert.IsNull(_store.GetForType<ITestClass>());
        }

        private class TestClass : ITestClass {}
        private interface ITestClass {}

        private class StoreClass
        {
            public int SpecialNumber { get; set; }
        }
    }
}