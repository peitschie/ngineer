using System;
using System.Linq;
using NUnit.Framework;
using NGineer.UnitTests.BuilderTests;
using NGineer.Utils;
using System.Collections.Generic;

namespace NGineer.UnitTests.BuilderTests
{
	[TestFixture]
	public class WithGeneratorTests
	{
		[Test]
		public void UniqueCollection_SyntaxExample()
		{
			var builder = new Builder(1)
				.SetCollectionSize(10,10)
				.WithGenerator(Generators.UniqueCollection<ClassWithEnumAndProperties, SimpleEnum>(c => c.EnumProperty));
			var list = builder.Build<List<ClassWithEnumAndProperties>>();
			Assert.AreEqual(EnumUtils.GetValues<SimpleEnum>().Count(), list.Count);
		}
	}
}
