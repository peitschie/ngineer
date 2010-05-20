using System;
using NGineer.BuildGenerators;
using NUnit.Framework;

namespace NGineer.UnitTests.BuildGenerators
{
    [TestFixture]
    public class BlittableTypesGeneratorTests : GeneratorTestFixture<BlittableTypesGenerator>
    {
        protected override Type[] SupportedTypes()
        {
            return new[]
                {
                    typeof(byte),
                    typeof(Byte),
                    typeof(sbyte),
                    typeof(SByte),

                    typeof (int),
                    typeof (Int32),
                    typeof (long),
                    typeof (double),
                    typeof (Double),
                    typeof (float),
                    typeof (uint),
                    typeof (ulong),
                    typeof (short),
                    typeof (ushort),
                };
        }

        protected override Type[] UnsupportedTypes()
        {
            return new[]
                {
                    typeof (char),
                    typeof (bool),
                    typeof(decimal),
                    typeof (object),
                    typeof (string),
                    
                };
        }
    }
}