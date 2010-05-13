using System;
using NGineer.Generators;
using NUnit.Framework;

namespace NGineer.UnitTests.Generators
{
    [TestFixture]
    public class EnumGeneratorTests : GeneratorTestFixture<EnumGenerator>
    {
        protected override Type[] SupportedTypes()
        {
            return new[]
                {
                    typeof (TestEnum),
                };
        }

        protected override Type[] UnsupportedTypes()
        {
            return new[]
                {
                    typeof (EnumGeneratorTests),
                    typeof (int),
                };
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