using Moq;
using NGineer.Internal;
using NUnit.Framework;
using Range = NGineer.Internal.Range;
using System;
using NGineer.UnitTests.BuildGenerators;
using NGineer.UnitTests.Types;

namespace NGineer.UnitTests.Internal
{
    [TestFixture]
    public class BuildSessionTests
    {
        private BuildSession _session;
        private TypeRegistry<Range> _registry;
        private Range _defaultRange;

        [SetUp]
        public void SetUp()
        {
            _defaultRange = new Range(10, 1000);
            _registry = new TypeRegistry<Range>();
        }

        [Test]
        public void GetCollectionSize_ReturnsDefaultIfNoneDefined()
        {
            var defRange = new Range(10, 1000);
            _session = new BuildSession(new TestBuilder{ CollectionSizes = _registry, DefaultCollectionSize = defRange }, (Random)null);
            Assert.AreSame(defRange, _session.GetCollectionSize(typeof(string)));
        }

        [Test]
        public void GetCollectionSize_ReturnsDefinedRange()
        {
            var stringRange = new Range(-10, 1); 
            var defRange = new Range(10, 1000);
            _registry.SetForType<string>(stringRange);
            _session = new BuildSession(new TestBuilder{ CollectionSizes = _registry, DefaultCollectionSize = defRange }, (Random)null);
            
            Assert.AreSame(stringRange, _session.GetCollectionSize(typeof(string)));
        }

        [Test]
        public void PushChild_SimpleObject()
        {
            var testObject = new object();
            _session = ConstructSession();
            var expectedParent = _session.CurrentObject;

            _session.PushObject(typeof(object), testObject);

            Assert.AreSame(testObject, _session.CurrentObject.Object);
            Assert.AreEqual(0, _session.BuildDepth);
            Assert.AreSame(expectedParent, _session.CurrentObject.Parent);
            Assert.AreEqual(1, _session.CurrentObject.Parent.Children.Count);
            Assert.AreSame(testObject, _session.CurrentObject.Parent.Children[0].Object);

            Assert.AreEqual(1, _session.ConstructedNodes.Count);
            Assert.AreSame(testObject, _session.ConstructedNodes[0].Object);
        }

        [Test]
        public void PushChild_NullObject()
        {
            _session = ConstructSession();
            var expectedParent = _session.CurrentObject;

            _session.PushObject(typeof(object), null);

            Assert.IsNull(_session.CurrentObject.Object);
            Assert.AreEqual(0, _session.BuildDepth);
            Assert.AreSame(expectedParent, _session.CurrentObject.Parent);
            Assert.AreEqual(1, _session.CurrentObject.Parent.Children.Count);
            Assert.IsNull(_session.CurrentObject.Parent.Children[0].Object);
            Assert.AreEqual(1, _session.ConstructedNodes.Count);
            Assert.IsNotNull(_session.ConstructedNodes[0]);
            Assert.IsNull(_session.ConstructedNodes[0].Object);
        }

        [Test]
        public void PushChild_NonNullParent_NonNullObject()
        {
            _session = ConstructSession();
            var parentObject = new object();
            var childObject = new object();

            _session.PushObject(typeof(object), parentObject);
            var expectedParent = _session.CurrentObject;
            _session.PushObject(typeof(object), childObject);

            Assert.AreSame(childObject, _session.CurrentObject.Object);
            Assert.AreSame(parentObject, _session.CurrentObject.Parent.Object);
            Assert.AreEqual(1, _session.BuildDepth);
            Assert.AreSame(expectedParent, _session.CurrentObject.Parent);
        }

        [Test]
        public void PushChild_NullParent_NonNullObject()
        {
            _session = ConstructSession();
            var childObject = new object();

            _session.PushObject(typeof(object), null);
            var expectedParent = _session.CurrentObject;
            _session.PushObject(typeof(object), childObject);

            Assert.AreSame(childObject, _session.CurrentObject.Object);
            Assert.AreEqual(1, _session.BuildDepth);
            Assert.AreSame(expectedParent, _session.CurrentObject.Parent);
        }

        [Test]
        public void PopChild_SinglePush_SinglePop()
        {
            _session = ConstructSession();

            _session.PushObject(typeof(object), new object());
            _session.PopObject();

            Assert.IsNull(_session.CurrentObject.Object);
            Assert.AreEqual(-1, _session.BuildDepth);
            Assert.IsNull(_session.CurrentObject.Parent);
        }

        [Test]
        public void PopChild_DoublePush_SinglePop()
        {
            var expectedObj = new object();
            var expectedChild = new object();
            _session = ConstructSession();
            var expectedParent = _session.CurrentObject;

            _session.PushObject(typeof(object), expectedObj);
            _session.PushObject(typeof(object), expectedChild);
            _session.PopObject();

            Assert.AreSame(expectedObj, _session.CurrentObject.Object);
            Assert.AreEqual(0, _session.BuildDepth);
            Assert.AreSame(expectedParent, _session.CurrentObject.Parent);
            Assert.AreEqual(1, _session.CurrentObject.Children.Count);
            Assert.AreSame(expectedChild, _session.CurrentObject.Children[0].Object);
        }

        private BuildSession ConstructSession()
        {
            return new BuildSession(new TestBuilder{ CollectionSizes = _registry, DefaultCollectionSize = _defaultRange }, (Random)null);
        }
    }
}