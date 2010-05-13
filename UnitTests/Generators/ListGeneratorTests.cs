using System.Collections.Generic;
using System;
using Moq;
using NGineer.BuildHelpers;
using NGineer.Generators;
using NUnit.Framework;
using Range = NGineer.BuildHelpers.Range;

namespace NGineer.UnitTests.Generators
{
    [TestFixture]
    public class ListGeneratorTests : GeneratorTestFixture<ListGenerator>
    {
        protected BuildSession NewSession()
        {
            return new BuildSession(new TypeRegistry<Range>(), new Range(10, 10));
        }

        [Test]
        public override void GeneratesTypes_AcceptsTypes()
        {
            Assert.IsTrue(GeneratesType(typeof(IList<string>)));
            Assert.IsTrue(GeneratesType(typeof(List<string>)));
            Assert.IsTrue(GeneratesType(typeof(IEnumerable<string>)));
        }
		
		[Test]
        public override void GeneratesTypes_RejectsTypes()
        {
            Assert.IsFalse(GeneratesType(typeof(string[])));
            Assert.IsFalse(GeneratesType(typeof(ListGeneratorTests)));
            Assert.IsFalse(GeneratesType(typeof(string)));
            Assert.IsFalse(GeneratesType(typeof(IDictionary<string, string>)));
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
            var session = new BuildSession(registry, new Range(10, 10));

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