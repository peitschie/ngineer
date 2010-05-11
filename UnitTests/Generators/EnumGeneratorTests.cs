using NGineer.Generators;
using NUnit.Framework;

namespace NGineer.UnitTests.Generators
{
    [TestFixture]
    public class EnumGeneratorTests : GeneratorTestFixture<EnumGenerator>
    {
		[Test]
        public override void GeneratesTypes_AcceptsTypes()
        {
            Assert.IsTrue(Generator.GeneratesType(typeof(TestEnum), null, null));
        }
		
		[Test]
        public override void GeneratesTypes_RejectsTypes()
        {
            Assert.IsFalse(Generator.GeneratesType(typeof(EnumGeneratorTests), null, null));
            Assert.IsFalse(Generator.GeneratesType(typeof(int), null, null));
        }
		
        [Test]
        public void GenerateSimpleEnum()
        {
            Generator = new EnumGenerator(2);
            var enumValue = Generator.Create(typeof(TestEnum), null, null);
            Assert.IsNotNull(enumValue);
            // I just happen to know that a seed of 2 will produce this result...
            Assert.AreEqual(TestEnum.Value3, enumValue);
        }

        public enum TestEnum
        {
            Value1,
            Value2,
            Value3
        }
    }
}