using System;
using NGineer.BuildGenerators;
using NUnit.Framework;

namespace NGineer.UnitTests.BuildGenerators
{
    [TestFixture]
    public class CharGeneratorTests : GeneratorTestFixture<CharGenerator>
    {
        protected override Type[] SupportedTypes()
        {
            return new[] {typeof (char)};
        }

        protected override Type[] UnsupportedTypes()
        {
            return new[]
                {
                    typeof (bool),
                    typeof (int),
                    typeof (string)
                };
        }
    }
}