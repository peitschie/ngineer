using NGineer.Generators;
using NUnit.Framework;

namespace NGineer.UnitTests.Generators
{
    [TestFixture]
    public class EnumGeneratorTests
    {
        private EnumGenerator _generator;

        [SetUp]
        public void SetUp()
        {
            _generator = new EnumGenerator(2);
        }

        [Test]
        public void GenerateSimpleEnum()
        {
            _generator = new EnumGenerator(2);
            var enumValue = _generator.Generate(typeof (TestEnum), null);
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