using System;
using System.Linq;
using NUnit.Framework;
using NGineer.BuildGenerators;
using System.Collections;
using System.Collections.Generic;
using NGineer.UnitTests.BuildGenerators;
using NGineer.Internal;
using Moq;
using NGineer.Utils;
using NGineer.UnitTests.BuilderTests;
using NGineer.UnitTests.Types;

namespace NGineer.UnitTests.BuildGenerators
{
    [TestFixture]
    public class UniqueCollectionGeneratorMemberTests
        : GeneratorTestFixture<UniqueCollectionGeneratorMember<ClassWithEnumAndProperties, SimpleEnum>>
    {
        protected override UniqueCollectionGeneratorMember<ClassWithEnumAndProperties, SimpleEnum> Construct ()
        {
            return new UniqueCollectionGeneratorMember<ClassWithEnumAndProperties, SimpleEnum>(c => c.EnumProperty);
        }

        protected override IBuilder GetBuilder()
        {
            var builder = new Mock<IBuilder>();
            builder.Setup(b => b.Build<ClassWithEnumAndProperties>())
                .Returns(() => new ClassWithEnumAndProperties());
            return builder.Object;
        }

        protected override Type[] SupportedTypes ()
        {
            return new []{
                typeof(ICollection<ClassWithEnumAndProperties>),
                typeof(IList<ClassWithEnumAndProperties>),
                typeof(IEnumerable<ClassWithEnumAndProperties>),
                typeof(List<ClassWithEnumAndProperties>),
            };
        }

        protected override Type[] UnsupportedTypes ()
        {
            return new[] {
                typeof(ICollection<int>),
                typeof(List<int>),
                typeof(string),
                typeof(IList),
                typeof(string[]),
                typeof(object)
            };
        }

        [Test]
        public void GenerateAndPopulate_CustomClassType()
        {
            var builderMock = new Mock<IBuilder>();
            builderMock.Setup(c => c.Build<ClassWithEnumAndProperties>())
                .Returns(() => new ClassWithEnumAndProperties(){EnumProperty = SimpleEnum.First});
            
            var list = CreateAndGenerate<IList<ClassWithEnumAndProperties>>(builderMock.Object, BuildSession());
            
            var expected = EnumUtils.GetValues<SimpleEnum>();
            Assert.AreEqual(Enum.GetValues(typeof(SimpleEnum)).Length, list.Count);
            foreach(var current in expected)
            {
                var entry = list.FirstOrDefault(c => c.EnumProperty == current);
                Assert.IsNotNull(entry, string.Format("No entry found for {0}", current));
                list.Remove(entry);
            }
            Assert.AreEqual(0, list.Count);
        }
    }
}
