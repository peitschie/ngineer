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
    public class ArrayGeneratorTests : GeneratorTestFixture<ArrayGenerator>
    {
        protected BuildSession NewSession()
        {
            return new BuildSession(null, new TypeRegistry<Range>(), new Range(10, 10));
        }

        protected override Type[] SupportedTypes()
        {
            return new[]
                {
                    typeof(string[]), 
                    typeof(int[]),
                };
        }

        protected override Type[] UnsupportedTypes()
        {
            return new[]
                {
                    typeof (List<string>),
                    typeof (IEnumerable<string>),
                    typeof (ListGeneratorTests),
                    typeof (string),
                    typeof (IDictionary<string, string>),
                };
        }

        [Test]
        public void Create_Simple_ArrayOfStrings()
        {
            var builderMock = new Mock<IBuilder>();
            builderMock.Setup(b => b.Build(typeof (string), It.IsAny<BuildSession>())).Returns("teststring");

            var list = CreateAndGenerate<string[]>(builderMock.Object, NewSession());
            Assert.AreEqual(10, list.Length);
            foreach (var result in list)
            {
                Assert.AreEqual("teststring", result);
            }
        }

        [Test]
        public void Create_Simple_ArrayOfComplexClassType()
        {
            var builderMock = new Mock<IBuilder>();
            builderMock.Setup(b => b.Build(typeof(ComplexClassType), It.IsAny<BuildSession>())).Returns(new ComplexClassType());

            var list = CreateAndGenerate<ComplexClassType[]>(builderMock.Object, NewSession());
            Assert.AreEqual(10, list.Length);
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
            var session = new BuildSession(null, registry, new Range(10, 10));

            var list = CreateAndGenerate<ComplexClassType[]>(builderMock.Object, session);
            Assert.AreEqual(20, list.Length);
            foreach (var result in list)
            {
                Assert.IsInstanceOf<ComplexClassType>(result);
                Assert.IsNotNull(result);
            }
        }

        [Test]
        public void Create_Simple_Nested_ArrayOfArrayOfComplexClassType()
        {
            var builderMock = new Mock<IBuilder>();
            builderMock.Setup(b => b.Build(typeof(ComplexClassType), It.IsAny<BuildSession>())).Returns(new ComplexClassType());
            builderMock.Setup(b => b.Build(typeof(ComplexClassType[]), It.IsAny<BuildSession>())).Returns(CreateAndGenerate<ComplexClassType[]>(builderMock.Object, NewSession()));

            var outerList = CreateAndGenerate<ComplexClassType[][]>(builderMock.Object, NewSession());
            Assert.AreEqual(10, outerList.Length);
            foreach (var innerList in outerList)
            {
                Assert.IsInstanceOf<ComplexClassType[]>(innerList);
                Assert.IsNotNull(innerList);
                Assert.AreEqual(10, innerList.Length);
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