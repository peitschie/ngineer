using System.Collections.Generic;
using System;
using Moq;
using NGineer.BuildHelpers;
using NGineer.Generators;
using NUnit.Framework;

namespace NGineer.UnitTests.Generators
{
    [TestFixture]
    public class ArrayGeneratorTests : GeneratorTestFixture<ArrayGenerator>
    {
        protected override ArrayGenerator Construct()
        {
            return new ArrayGenerator(1, 10, 10);
        }

        [Test]
        public override void GeneratesTypes_AcceptsTypes()
        {
            Assert.IsTrue(GeneratesType(typeof(string[])));
            Assert.IsTrue(GeneratesType(typeof(int[])));
        }
		
		[Test]
        public override void GeneratesTypes_RejectsTypes()
        {
            Assert.IsFalse(GeneratesType(typeof(List<string>)));
            Assert.IsFalse(GeneratesType(typeof(IEnumerable<string>)));
            Assert.IsFalse(GeneratesType(typeof(ListGeneratorTests)));
            Assert.IsFalse(GeneratesType(typeof(string)));
            Assert.IsFalse(GeneratesType(typeof(IDictionary<string, string>)));
        }
		
        [Test]
        public void Create_Simple_ArrayOfStrings()
        {
            var builderMock = new Mock<IBuilder>();
            builderMock.Setup(b => b.Build(typeof (string), It.IsAny<BuildSession>())).Returns("teststring");

            var list = CreateAndGenerate<string[]>(builderMock.Object, null);
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

            var list = CreateAndGenerate<ComplexClassType[]>(builderMock.Object, null);
            Assert.AreEqual(10, list.Length);
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
            builderMock.Setup(b => b.Build(typeof(ComplexClassType[]), It.IsAny<BuildSession>())).Returns(CreateAndGenerate<ComplexClassType[]>(builderMock.Object, null));

            var outerList = CreateAndGenerate<ComplexClassType[][]>(builderMock.Object, null);
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