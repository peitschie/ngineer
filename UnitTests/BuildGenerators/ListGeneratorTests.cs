using System.Collections.Generic;
using System;
using Moq;
using NGineer.BuildGenerators;
using NGineer.Internal;
using NUnit.Framework;
using Range = NGineer.Internal.Range;
using NGineer.UnitTests.Types;

namespace NGineer.UnitTests.BuildGenerators
{
    [TestFixture]
    public class ListGeneratorTests : GeneratorTestFixture<ListGenerator>
    {
        protected BuildSession NewSession()
        {
            return new BuildSession(new TestBuilder{
                CollectionSizes = new TypeRegistry<Range>(),
                DefaultCollectionSize = new Range(10, 10),
                BuildDepth = 10
            }, null, new Random(10));
        }

        protected override Type[] SupportedTypes()
        {
            return new[]
                {
                    typeof (IList<string>),
                    typeof (List<string>),
                    typeof (IEnumerable<string>),
                };
        }

        protected override Type[] UnsupportedTypes()
        {
            return new[]
                {
                    typeof(CustomListType),
                    typeof(CustomListTypeGeneric<string>),
                    typeof (string[]),
                    typeof (ListGeneratorTests),
                    typeof (string),
                    typeof (IDictionary<string, string>),
                };
        }

        protected override IBuilder GetBuilder()
        {
            var builder = new Mock<IBuilder>();
            builder.Setup(b => b.Build(It.IsAny<Type>())).Returns(null);
            return builder.Object;
        }

        [Test]
        public void Create_Simple_FixedSizedListOfStrings()
        {
            var builderMock = new Mock<IBuilder>();
            builderMock.Setup(b => b.Build(typeof (string))).Returns("teststring");

            var list = CreateAndGenerate<IList<string>>(builderMock.Object, NewSession());
            Assert.AreEqual(10, list.Count);
            foreach (var result in list)
            {
                Assert.AreEqual("teststring", result);
            }
        }

        [Test]
        public void Create_Simple_FixedSizedListOfComplexClassType()
        {
            var builderMock = new Mock<IBuilder>();
            builderMock.Setup(b => b.Build(typeof(ComplexClassType))).Returns(new ComplexClassType());

            var list = CreateAndGenerate<IList<ComplexClassType>>(builderMock.Object, NewSession());
            Assert.AreEqual(10, list.Count);
            foreach (var result in list)
            {
                Assert.IsInstanceOf<ComplexClassType>(result);
                Assert.IsNotNull(result);
            }
        }

        [Test]
        public void Create_ChecksCollectionSizeForComplexClassType()
        {
            var builderMock = new Mock<IBuilder>();
            builderMock.Setup(b => b.Build(typeof(ComplexClassType))).Returns(new ComplexClassType());
            var registry = new TypeRegistry<Range>();
            registry.SetForType<ComplexClassType>(new Range(20, 20));
            var session = new BuildSession(new TestBuilder {
                CollectionSizes = registry,
                DefaultCollectionSize = new Range(10, 10),
                BuildDepth = 10
            }, null, new Random(10));

            var list = CreateAndGenerate<IList<ComplexClassType>>(builderMock.Object, session);
            Assert.AreEqual(20, list.Count);
            foreach (var result in list)
            {
                Assert.IsInstanceOf<ComplexClassType>(result);
                Assert.IsNotNull(result);
            }
        }

        [Test]
        public void Create_Simple_NestedFixedSizedListOfComplexClassType()
        {
            var builderMock = new Mock<IBuilder>();
            builderMock.Setup(b => b.Build(typeof(ComplexClassType))).Returns(new ComplexClassType());
            builderMock.Setup(b => b.Build(typeof(IList<ComplexClassType>))).Returns(CreateAndGenerate<IList<ComplexClassType>>(builderMock.Object, NewSession()));

            var outerList = CreateAndGenerate<IList<IList<ComplexClassType>>>(builderMock.Object, NewSession());
            Assert.AreEqual(10, outerList.Count);
            foreach (var innerList in outerList)
            {
                Assert.IsInstanceOf<IList<ComplexClassType>>(innerList);
                Assert.IsNotNull(innerList);
                Assert.AreEqual(10, innerList.Count);
                foreach (var innerResult in innerList)
                {
                    Assert.IsInstanceOf<ComplexClassType>(innerResult);
                    Assert.IsNotNull(innerResult);
                }
            }
        }

        public class ComplexClassType {}

        public class CustomListType : List<string> {}

        public class CustomListTypeGeneric<TType> : List<TType> {}
    }
}