using System;
using System.Linq;
using NUnit.Framework;
using NGineer.UnitTests.BuilderTests;
using NGineer.Utils;
using System.Collections.Generic;
using NGineer.UnitTests.Types;

namespace NGineer.UnitTests.BuilderTests
{
	[TestFixture]
	public class WithGeneratorTests
	{
        private IBuilder _builder;

        [SetUp]
        public void SetUp()
        {
            _builder = new Builder();
        }

		[Test]
		public void UniqueCollection_SyntaxExample_UsingGenerator()
		{
			_builder.SetDefaultCollectionSize(10,10)
				.WithGenerator(Generators.UniqueCollection.ForMember<ClassWithEnumAndProperties>(c => c.EnumProperty));

            var list = _builder.Build<List<ClassWithEnumAndProperties>>();

            Assert.AreEqual(EnumUtils.GetValues<SimpleEnum>().Count(), list.Count);
		}
	    
        [Test]
		public void CustomGeneratedObject_IsNotThenPopulated()
		{
            _builder.WithGenerator(() => new ThrowsOnAccess());
            var result = _builder.Build<ThrowsOnAccess>();
            Assert.IsNotNull(result);
            Assert.Throws<Exception>(() => Console.Out.Write(result.Property));
            Assert.IsNull(result.Property2);
		}

        private class ThrowsOnAccess
        {
            public int Property
            {
                get { throw new Exception("Property should not have been accessed"); }
                set { throw new Exception("Property should not have been accessed"); }
            }

            public string Property2 { get; set; }
        }
	}
}
