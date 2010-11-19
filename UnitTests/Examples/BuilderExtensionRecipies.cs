using System.Collections.Generic;
using NGineer.UnitTests.Types;
using NUnit.Framework;

namespace NGineer.UnitTests.Examples
{
    [TestFixture]
    public class BuilderExtensionRecipies
    {
        private IBuilder _builder;

        [SetUp]
        public void SetUp()
        {
            _builder = new Builder();
        }

        [Test]
        public void SetKey_DictionaryType()
        {
            var dictionary = _builder
                .For<Dictionary<string, string>>().SetKey("demonstration", "value")
                .Build<Dictionary<string, string>>();

            Assert.IsTrue(dictionary.ContainsKey("demonstration"));
            Assert.AreEqual("value",dictionary["demonstration"]);
        }

        [Test]
        public void SetKey_ObjectWithDictionaryProperty()
        {
            var instance = _builder
                .For<ClassWithDictionary>().SetKey(x => x.Dictionary, "demonstration", "value")
                .Build<ClassWithDictionary>();

            Assert.IsTrue(instance.Dictionary.ContainsKey("demonstration"));
            Assert.AreEqual("value", instance.Dictionary["demonstration"]);
        }
    }
}