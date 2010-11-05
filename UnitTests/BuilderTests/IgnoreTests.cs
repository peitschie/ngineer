using NUnit.Framework;

namespace NGineer.UnitTests.BuilderTests
{
    [TestFixture]
    public class IgnoreTests
    {
        private IBuilder _builder;

        [SetUp]
        public void SetUp()
        {
            _builder = new Builder();
        }

        [Test]
        public void IgnoreUnset_DoesntSetUnsetMembers()
        {
            var simpleClass = _builder
                .For<SimpleClass>()
                    .IgnoreUnset()
                    .Set(c => c.IntProperty, 10)
                    .Set(c => c.StringField, "12fda")
                    .Ignore(c => c.TestClass2Field)
                .Build<SimpleClass>();
            Assert.IsNull(simpleClass.TestClass2Field);
            Assert.AreEqual(10, simpleClass.IntProperty);
            Assert.AreEqual("12fda", simpleClass.StringField);
            Assert.AreEqual(default(int), simpleClass.IntField);
            Assert.AreEqual(default(string), simpleClass.StringProperty);
            Assert.AreEqual(default(TestClass2), simpleClass.TestClass2Property);
        }

        [Test]
        public void IgnoreUnset_StillPerformsPostPopulators()
        {
            var populatorCalled = false;
            _builder
                .For<SimpleClass>()
                    .IgnoreUnset()
                    .Set(c => c.IntProperty, 10)
                    .Set(c => c.StringField, "12fda")
                    .Do(c => populatorCalled = true)
                .Build<SimpleClass>();
            Assert.IsTrue(populatorCalled);
        }

        [Test]
        public void IgnoreUnset_ChildBuilder_DoesntSetUnsetMembers()
        {
            var simpleClass = _builder
                .For<SimpleClass>()
                    .IgnoreUnset()
                    .Set(c => c.IntProperty, 10)
                    .Set(c => c.StringField, "12fda")
                    .Ignore(c => c.TestClass2Field)
                .CreateNew()
                .Build<SimpleClass>();
            Assert.IsNull(simpleClass.TestClass2Field);
            Assert.AreEqual(10, simpleClass.IntProperty);
            Assert.AreEqual("12fda", simpleClass.StringField);
            Assert.AreEqual(default(int), simpleClass.IntField);
            Assert.AreEqual(default(string), simpleClass.StringProperty);
            Assert.AreEqual(default(TestClass2), simpleClass.TestClass2Property);
        }

        [Test]
        public void IgnoreAll_OverridesPreviousSetters()
        {
            var simpleClass = _builder
                .For<SimpleClass>()
                    .Set(c => c.IntProperty, 10)
                    .Set(c => c.StringField, "12fda")
                    .IgnoreAll()
                    .Set(c => c.IntField, 10)
                    .Ignore(c => c.TestClass2Field)
                .Build<SimpleClass>();
            Assert.AreEqual(default(int), simpleClass.IntProperty);
            Assert.AreEqual(default(string), simpleClass.StringField);
            Assert.AreEqual(10, simpleClass.IntField);
            Assert.AreEqual(default(string), simpleClass.StringProperty);
            Assert.AreEqual(default(TestClass2), simpleClass.TestClass2Property);
            Assert.AreEqual(default(TestClass2), simpleClass.TestClass2Field);
        }

        [Test]
        public void IgnoreAll_OverridesPreviousSetters_AllowsResetting()
        {
            var simpleClass = _builder
                .For<SimpleClass>()
                    .Set(c => c.IntProperty, 10)
                    .IgnoreAll()
                    .Set(c => c.IntProperty, 12)
                .Build<SimpleClass>();
            Assert.AreEqual(12, simpleClass.IntProperty);
            Assert.AreEqual(default(int), simpleClass.IntField);
            Assert.AreEqual(default(string), simpleClass.StringField);
            Assert.AreEqual(default(string), simpleClass.StringProperty);
            Assert.AreEqual(default(TestClass2), simpleClass.TestClass2Property);
            Assert.AreEqual(default(TestClass2), simpleClass.TestClass2Field);
        }

        [Test]
        public void IgnoreAll_OverridesPreviousSetters_BehavesCorrectlyInChildBuilder()
        {
            var simpleClass = _builder
                .For<SimpleClass>()
                    .Set(c => c.IntProperty, 10)
                    .Set(c => c.StringField, "12fda")
                .CreateNew()
                    .For<SimpleClass>()
                        .IgnoreAll()
                        .Set(c => c.IntField, 10)
                        .Ignore(c => c.TestClass2Field)
                .Build<SimpleClass>();
            Assert.AreEqual(default(int), simpleClass.IntProperty);
            Assert.AreEqual(default(string), simpleClass.StringField);
            Assert.AreEqual(10, simpleClass.IntField);
            Assert.AreEqual(default(string), simpleClass.StringProperty);
            Assert.AreEqual(default(TestClass2), simpleClass.TestClass2Property);
            Assert.AreEqual(default(TestClass2), simpleClass.TestClass2Field);
        }
    }
}