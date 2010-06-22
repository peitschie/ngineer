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
			// can't think of a reliable way to tell wat the produced results will be...
        }

		[Test]
        public void GenerateSimpleEnum_ValueChanges()
        {
			var valueChanged = false;
			var lastValue = TestEnum.Value1;
			const int maxTries = 10;
            Generator = new EnumGenerator();
			var session = BuildSession();
			for(int tries = 0; tries < maxTries; tries++)
			{
            	var enumValue = (TestEnum)Generator.Create(typeof(TestEnum), null, session);
            	Assert.IsNotNull(enumValue);
				if(tries > 0 && lastValue != enumValue)
				{
					valueChanged = true;
					break;
				}
				lastValue = enumValue;
			}
			Assert.IsTrue(valueChanged, "EnumGenerator always returns identical value");
        }
		
        public enum TestEnum
        {
            Value1,
            Value2,
            Value3
        }
    }
}