
using System;
using NUnit.Framework;
using NGineer.Generators;

namespace NGineer.UnitTests.Generators
{

	[TestFixture]
	public class BuilderGeneratorTests
	{

		[Test]
		public void Constructor_GenerateString()
		{
			var generator = new BuilderGenerator<string>(1, () => "teststring");
			Assert.AreEqual("teststring", generator.Create(typeof(string), null, null));
		}
	}
}
