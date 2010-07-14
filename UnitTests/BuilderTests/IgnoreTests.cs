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
    }
}