using System.Collections.Generic;
using System;
using Moq;
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
            Assert.IsTrue(_generator.GeneratesType(typeof(IList<string>), null));
			Assert.IsTrue(_generator.GeneratesType(typeof(List<string>), null));
        }
		
		[Test]
        public override void GeneratesTypes_RejectsTypes()
        {
            Assert.IsFalse(_generator.GeneratesType(typeof(ListGeneratorTests), null));
			Assert.IsFalse(_generator.GeneratesType(typeof(string), null));
			Assert.IsFalse(_generator.GeneratesType(typeof(IDictionary<string, string>), null));
        }
		
        [Test]
        public void Create_Simple_FixedSizedListOfStrings()
        {
            var builderMock = new Mock<IBuilder>();
            builderMock.Setup(b => b.Build(typeof (string))).Returns("teststring");

            var list = (IList<string>)_generator.Generate(typeof (IList<string>), builderMock.Object);
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

            var list = (IList<ComplexClassType>)_generator.Generate(typeof(IList<ComplexClassType>), builderMock.Object);
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
            builderMock.Setup(b => b.Build(typeof(ComplexClassType))).Returns(new ComplexClassType());
            builderMock.Setup(b => b.Build(typeof(IList<ComplexClassType>))).Returns(_generator.Generate(typeof(IList<ComplexClassType>), builderMock.Object));

            var outerList = (IList<IList<ComplexClassType>>)_generator.Generate(typeof(IList<IList<ComplexClassType>>), builderMock.Object);
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