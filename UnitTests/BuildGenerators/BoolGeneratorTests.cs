using System;
using NGineer.BuildGenerators;
using NUnit.Framework;

namespace NGineer.UnitTests.BuildGenerators
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