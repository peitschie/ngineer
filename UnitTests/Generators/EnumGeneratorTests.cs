using NGineer.Generators;
using NUnit.Framework;

namespace NGineer.UnitTests.Generators
{
    [TestFixture]
    public class EnumGeneratorTests : GeneratorTestFixture
    {
        private EnumGenerator _generator;

        [SetUp]
        public void SetUp()
        {
            _generator = new EnumGenerator(2);
        }

		[Test]
        public override void GeneratesTypes_AcceptsTypes()
        {
            Assert.IsTrue(_generator.GeneratesType(typeof(TestEnum), null, null));
        }
		
		[Test]
        public override void GeneratesTypes_RejectsTypes()
        {
            Assert.IsFalse(_generator.GeneratesType(typeof(EnumGeneratorTests), null, null));
            Assert.IsFalse(_generator.GeneratesType(typeof(int), null, null));
        }
		
        [Test]
        public void GenerateSimpleEnum()
        {
            _generator = new EnumGenerator(2);
            var enumValue = _generator.Create(typeof(TestEnum), null, null);
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