using System.Collections.Generic;
using System;
using Moq;
using NGineer.BuildHelpers;
using NGineer.Generators;
using NUnit.Framework;

namespace NGineer.UnitTests.Generators
{
    [TestFixture]
    public class ListGeneratorTests : GeneratorTestFixture
    {
        private ListGenerator _generator;

        [SetUp]
        public void SetUp()
        {
            _generator = new ListGenerator(1, 10, 10);
        }
	
        [Test]
        public override void GeneratesTypes_AcceptsTypes()
        {
            Assert.IsTrue(_generator.GeneratesType(typeof(IList<string>), null, null));
            Assert.IsTrue(_generator.GeneratesType(typeof(List<string>), null, null));
        }
		
		[Test]
        public override void GeneratesTypes_RejectsTypes()
        {
            Assert.IsFalse(_generator.GeneratesType(typeof(ListGeneratorTests), null, null));
            Assert.IsFalse(_generator.GeneratesType(typeof(string), null, null));
            Assert.IsFalse(_generator.GeneratesType(typeof(IDictionary<string, string>), null, null));
        }
		
        [Test]
        public void Create_Simple_FixedSizedListOfStrings()
        {
            var builderMock = new Mock<IBuilder>();
            builderMock.Setup(b => b.Build(typeof (string), It.IsAny<BuildSession>())).Returns("teststring");

            var list = (IList<string>)_generator.Create(typeof(IList<string>), builderMock.Object, null);
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

            var list = (IList<ComplexClassType>)_generator.Create(typeof(IList<ComplexClassType>), builderMock.Object, null);
            Assert.AreEqual(10, list.Count);
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
            builderMock.Setup(b => b.Build(typeof(IList<ComplexClassType>), It.IsAny<BuildSession>())).Returns(_generator.Create(typeof(IList<ComplexClassType>), builderMock.Object, null));

            var outerList = (IList<IList<ComplexClassType>>)_generator.Create(typeof(IList<IList<ComplexClassType>>), builderMock.Object, null);
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