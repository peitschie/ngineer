using System.Collections.Generic;
using NGineer.UnitTests.Types;
using NUnit.Framework;
using System.Linq;

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

        #region Build_WithGenerator_ConcreteTypesInPlaceOfAbstractTypes
        public abstract class AbstractParent
        {
            public string ParentProperty { get; set; }
        }

        public class Child1 : AbstractParent
        {
            public string Child1Property { get; set; }
        }

        public class Child2 : AbstractParent
        {
            public string Child2Property { get; set; }
        }

        [Test]
        public void Build_WithGenerator_ConcreteTypesInPlaceOfAbstractTypes()
        {
            var instances = _builder
                                .SetCollectionSize<AbstractParent>(5,10)
                                .WithGenerator<AbstractParent>(typeof(Child1), typeof(Child2))
                                .Build<AbstractParent[]>();

            Assert.That(instances.OfType<Child1>().Count(), Is.GreaterThan(0));
            foreach(var child1 in instances.OfType<Child1>())
            {
                Assert.IsNotNull(child1.ParentProperty);
                Assert.IsNotNull(child1.Child1Property);
            }

            Assert.That(instances.OfType<Child2>().Count(), Is.GreaterThan(0));
            foreach(var child2 in instances.OfType<Child2>())
            {
                Assert.IsNotNull(child2.ParentProperty);
                Assert.IsNotNull(child2.Child2Property);
            }
        }
        #endregion
    }
}