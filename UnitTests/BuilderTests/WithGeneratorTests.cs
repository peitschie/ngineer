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
	    private IBuilder _builder;

        [SetUp]
        public void SetUp()
        {
            _builder = new Builder();
        }

        [Test]
		public void CustomGeneratedObject_IsNotThenPopulated()
		{
            _builder.WithGenerator(() => new ThrowsOnAccess());
            ThrowsOnAccess result = null;
            result = _builder.Build<ThrowsOnAccess>();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Property2);
		}

        private class ThrowsOnAccess
        {
            private int _property;
            public int Property
            {
                get { throw new Exception("Property should not have been accessed"); }
                set { throw new Exception("Property should not have been accessed"); }
            }

            public string Property2 { get; set; }
        }
	}
}
