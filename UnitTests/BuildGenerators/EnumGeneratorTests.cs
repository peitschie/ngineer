using System;
using NGineer.BuildGenerators;
using NUnit.Framework;

namespace NGineer.UnitTests.BuildGenerators
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
            Generator = new EnumGenerator();
            var enumValue = Generator.Create(typeof(TestEnum), null, BuildSession());
            Assert.IsNotNull(enumValue);
            // I just happen to know that a seed of 2 will produce this result...
            Assert.AreEqual(TestEnum.Value2, enumValue);
        }

        public enum TestEnum
        {
            Value1,
            Value2,
            Value3
        }
    }
}