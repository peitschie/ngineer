using System;
using System.Linq;
using NUnit.Framework;
using NGineer.BuildGenerators;
using System.Collections;
using System.Collections.Generic;
using NGineer.UnitTests.BuildGenerators;
using NGineer.BuildHelpers;
using Moq;
using NGineer.Utils;
using NGineer.UnitTests.BuilderTests;

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
            builderMock.Setup(c => c.Build<ClassWithEnumAndProperties>(It.IsAny<BuildSession>()))
                .Returns(() => new ClassWithEnumAndProperties(){EnumProperty = SimpleEnum.First});
            
            var list = CreateAndGenerate<IList<ClassWithEnumAndProperties>>(builderMock.Object, BuildSession());
            
            var expected = EnumUtils.GetValues<SimpleEnum>();
            Assert.AreEqual(Enum.GetValues(typeof(SimpleEnum)).Length, list.Count);
            foreach(var current in expected)
            {
                var entry = list.FirstOrDefault(c => c.EnumProperty == current);
                Assert.IsNotNull(entry, "No entry found for {0}".With(current));
                list.Remove(entry);
            }
            Assert.AreEqual(0, list.Count);
        }
    }
}
