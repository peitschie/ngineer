using System.Collections.Generic;
using System;
using Moq;
using NGineer.BuildHelpers;
using NGineer.Generators;
using NUnit.Framework;

namespace NGineer.UnitTests.Generators
{
    [TestFixture]
    public class NullableTypeGeneratorTests : GeneratorTestFixture<NullableTypeGenerator>
    {

        [Test]
        public override void GeneratesTypes_AcceptsTypes()
        {
            Assert.IsTrue(GeneratesType(typeof(Nullable<int>)));
            Assert.IsTrue(GeneratesType(typeof(DateTime?)));
        }
		
		[Test]
        public override void GeneratesTypes_RejectsTypes()
        {
            Assert.IsFalse(GeneratesType(typeof(int)));
            Assert.IsFalse(GeneratesType(typeof(string)));
        }

        [Test]
        public void Occasionally_Returns_Null()
        {
            var builder = new Mock<IBuilder>();
            builder.Setup(b => b.Build(typeof (int), It.IsAny<BuildSession>())).Returns(10);

            bool someNull = false;
            bool someNotNull = false;

            for(int i = 0; i < 20 && !(someNotNull && someNull); i++)
            {
                var result = CreateAndGenerate<int?>(builder.Object, null);
                if (result == null)
                    someNull = true;
                else
                    someNotNull = true;
            }
            Assert.IsTrue(someNull, "No numbers were null");
            Assert.IsTrue(someNotNull, "All numbers were null");
        }
    }
}