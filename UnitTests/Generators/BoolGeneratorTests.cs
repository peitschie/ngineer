using System;
using NGineer.Generators;
using NUnit.Framework;

namespace NGineer.UnitTests.Generators
{
    [TestFixture]
    public class BoolGeneratorTests : GeneratorTestFixture<BoolGenerator>
    {
        protected override Type[] SupportedTypes()
        {
            return new[] {typeof (bool)};
        }

        protected override Type[] UnsupportedTypes()
        {
            return new[]
                {
                    typeof (int),
                    typeof (string)
                };
        }
    }
}