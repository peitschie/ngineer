using System;
using NGineer.BuildGenerators;
using NUnit.Framework;

namespace NGineer.UnitTests.BuildGenerators
{
	[TestFixture]
	public class BuilderGeneratorTests : GeneratorTestFixture<BuilderGenerator>
	{
	    private const string TestString = "teststring";

        protected override BuilderGenerator Construct()
        {
            return new BuilderGenerator(typeof(string), (b, s) => TestString);
        }

	    protected override Type[] SupportedTypes()
	    {
	        return new[] {typeof (string)};
	    }

	    protected override Type[] UnsupportedTypes()
	    {
            return new[] { typeof(int), typeof(object), typeof(long) };
	    }

        [Test]
        public void Constructor_GenerateString()
        {
            Assert.AreEqual(TestString, Generator.CreateRecord(typeof(string), null, null).Object);
        }
	}

    [TestFixture]
    public class BuilderGeneratorGenericsTests : GeneratorTestFixture<BuilderGenerator<string>>
    {
        private const string TestString = "teststring";

        protected override BuilderGenerator<string> Construct()
        {
            return new BuilderGenerator<string>((b, s) => TestString);
        }

        protected override Type[] SupportedTypes()
        {
            return new[] { typeof(string) };
        }

        protected override Type[] UnsupportedTypes()
        {
            return new[] { typeof(int), typeof(object), typeof(long) };
        }

        [Test]
        public void Constructor_GenerateString()
        {
            Assert.AreEqual(TestString, Generator.CreateRecord(typeof(string), null, null).Object);
        }
    }
}
