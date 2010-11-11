using System;
using Moq;
using NGineer.BuildGenerators;
using NGineer.Internal;
using NUnit.Framework;

namespace NGineer.UnitTests.BuildGenerators
{
    [TestFixture]
    public class NullableTypeGeneratorTests : GeneratorTestFixture<NullableTypeGenerator>
    {
        protected override Type[] SupportedTypes()
        {
            return new[]
                {
                    typeof (Nullable<int>),
                    typeof (DateTime?),
                };
        }

        protected override Type[] UnsupportedTypes()
        {
            return new[]
                {
                    typeof (int),
                    typeof (string),
                };
        }

        [Test]
        public void Occasionally_Returns_Null()
        {
            var builder = new Mock<IBuilder>();
            builder.Setup(b => b.Build(typeof (int), It.IsAny<BuildSession>())).Returns(10);

            bool someNull = false;
            bool someNotNull = false;

			var session = BuildSession();
            for(int i = 0; i < 20 && !(someNotNull && someNull); i++)
            {
                var result = CreateAndGenerate<int?>(builder.Object, session);
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