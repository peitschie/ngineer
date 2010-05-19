using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using Moq;
using NGineer.BuildGenerators;
using NGineer.BuildHelpers;
using NUnit.Framework;

namespace NGineer.UnitTests.Generators
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