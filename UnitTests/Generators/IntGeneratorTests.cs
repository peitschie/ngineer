using System.Collections.Generic;
using System;
using Moq;
using NGineer.BuildHelpers;
using NGineer.Generators;
using NUnit.Framework;

namespace NGineer.UnitTests.Generators
{
    [TestFixture]
    public class IntGeneratorTests : GeneratorTestFixture<IntGenerator>
    {

        [Test]
        public override void GeneratesTypes_AcceptsTypes()
        {
            Assert.IsTrue(GeneratesType(typeof(int)));
            Assert.IsTrue(GeneratesType(typeof(Int32)));
        }
		
		[Test]
        public override void GeneratesTypes_RejectsTypes()
        {
            Assert.IsFalse(GeneratesType(typeof(long)));
            Assert.IsFalse(GeneratesType(typeof(double)));
        }
    }
}