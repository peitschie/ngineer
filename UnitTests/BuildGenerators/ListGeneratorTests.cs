using System.Collections.Generic;
using System;
using Moq;
using NGineer.BuildGenerators;
using NGineer.BuildHelpers;
using NUnit.Framework;
using Range = NGineer.BuildHelpers.Range;

namespace NGineer.UnitTests.BuildGenerators
{
    [TestFixture]
    public class ListGeneratorTests : GeneratorTestFixture<ListGenerator>
    {
        protected BuildSession NewSession()
        {
            return new BuildSession(null, new TypeRegistry<Range>(), null, new Range(10, 10), new Random(10));
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
                    typeof (string[]),
                    typeof (ListGeneratorTests),
                    typeof (string),
                    typeof (IDictionary<string, string>),
                };
        }

        [Test]
        public void Create_Simple_FixedSizedListOfStrings()
        {
            var builderMock = new Mock<IBuilder>();
            builderMock.Setup(b => b.Build(typeof (string), It.IsAny<BuildSession>())).Returns("teststring");

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
            builderMock.Setup(b => b.Build(typeof(ComplexClassType), It.IsAny<BuildSession>())).Returns(new ComplexClassType());

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
            builderMock.Setup(b => b.Build(typeof(ComplexClassType), It.IsAny<BuildSession>())).Returns(new ComplexClassType());
            var registry = new TypeRegistry<Range>();
            registry.SetForType<ComplexClassType>(new Range(20, 20));
            var session = new BuildSession(null, registry, null, new Range(10, 10), new Random(10));

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
            builderMock.Setup(b => b.Build(typeof(ComplexClassType), It.IsAny<BuildSession>())).Returns(new ComplexClassType());
            builderMock.Setup(b => b.Build(typeof(IList<ComplexClassType>), It.IsAny<BuildSession>())).Returns(CreateAndGenerate<IList<ComplexClassType>>(builderMock.Object, NewSession()));

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
    }
}