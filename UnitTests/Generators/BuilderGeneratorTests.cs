
using System;
using NGineer.BuildGenerators;
using NUnit.Framework;

namespace NGineer.UnitTests.Generators
{

	[TestFixture]
	public class BuilderGeneratorTests
	{

		[Test]
		public void Constructor_GenerateString()
		{
			var generator = new BuilderGenerator<string>((b, s) => "teststring");
			Assert.AreEqual("teststring", generator.Create(typeof(string), null, null));
		}
	}
}
