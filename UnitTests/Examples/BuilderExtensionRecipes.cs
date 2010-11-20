using System.Collections.Generic;
using NGineer.UnitTests.Types;
using NUnit.Framework;

namespace NGineer.UnitTests.Examples
{
    [TestFixture]
    public class BuilderExtensionRecipes
    {
        private IBuilder _builder;

        [SetUp]
        public void SetUp()
        {
            _builder = new Builder();
        }

        [Test]
        public void AddEntry_DictionaryType()
        {
            var dictionary = _builder
                .For<Dictionary<string, string>>().AddEntry("demonstration", "value")
                .Build<Dictionary<string, string>>();

            Assert.IsTrue(dictionary.ContainsKey("demonstration"));
            Assert.AreEqual("value",dictionary["demonstration"]);
        }

        [Test]
        public void AddEntry_ObjectWithDictionaryProperty()
        {
            var instance = _builder
                .For<ClassWithDictionary>().AddEntry(x => x.Dictionary, "demonstration", "value")
                .Build<ClassWithDictionary>();

            Assert.IsTrue(instance.Dictionary.ContainsKey("demonstration"));
            Assert.AreEqual("value", instance.Dictionary["demonstration"]);
        }

        [Test]
        public void AddEntry_ListType()
        {
            var list = _builder.For<List<string>>().AddEntry("demonstration").Build<List<string>>();

            Assert.IsTrue(list.Contains("demonstration"));
        }

        [Test]
        public void AddEntry_ObjectWithListProperty()
        {
            var instance = _builder.For<ClassWithList>().AddEntry(x => x.List, "demonstration").Build<ClassWithList>();

            Assert.IsTrue(instance.List.Contains("demonstration"));
        }
    }
}