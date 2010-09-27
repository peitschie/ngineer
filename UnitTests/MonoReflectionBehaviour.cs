using System;
using NUnit.Framework;
using NGineer.UnitTests.BuilderTests;
namespace NGineer.UnitTests
{
	[TestFixture]
	public class MonoReflectionBehaviour
	{
		[Test]
		public void A_ReflectedType_CacheReturnsStaleData_Initial()
		{
			var siblingProperty = typeof(InheritsFromClassWithNullableDateTime).GetProperty("Property1");
			var parentProperty = typeof(ClassWithNullableDateTime).GetProperty("Property1");
			
			Assert.AreEqual(typeof(InheritsFromClassWithNullableDateTime), siblingProperty.ReflectedType);
			Assert.AreEqual(typeof(ClassWithNullableDateTime), parentProperty.ReflectedType);
			
			Assert.AreEqual(typeof(ClassWithNullableDateTime), parentProperty.DeclaringType);
			Assert.AreEqual(typeof(ClassWithNullableDateTime), siblingProperty.DeclaringType);
		}

        /// <summary>
        /// This bug will fail if https://bugzilla.novell.com/show_bug.cgi?id=633671 is currently
        /// present in the executing CLR runtime
        /// </summary>
		[Test]
		public void ReflectedType_CacheReturnsStaleData()
		{
			var siblingProperty = typeof(InheritsFromClassWithNullableDateTime).GetProperty("Property1");
			var sibling2Property = typeof(InheritsFromClassWithNullableDateTime2).GetProperty("Property1");
			var parentProperty = typeof(ClassWithNullableDateTime).GetProperty("Property1");
			
			Assert.AreEqual(typeof(InheritsFromClassWithNullableDateTime), siblingProperty.ReflectedType);
			Assert.AreEqual(typeof(InheritsFromClassWithNullableDateTime2), sibling2Property.ReflectedType);
			Assert.AreEqual(typeof(ClassWithNullableDateTime), parentProperty.ReflectedType);
			
			Assert.AreEqual(typeof(ClassWithNullableDateTime), parentProperty.DeclaringType);
			Assert.AreEqual(typeof(ClassWithNullableDateTime), siblingProperty.DeclaringType);
			Assert.AreEqual(typeof(ClassWithNullableDateTime), sibling2Property.DeclaringType);
		}
	}
}

