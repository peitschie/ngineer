using System;
using System.Linq;
using NUnit.Framework;


namespace NGineer.UnitTests
{
	[TestFixture]
	public class BuilderTests
	{
		[Test]
		public void Builder_LeavesNoTypeUnconstructed()
		{
			var newClass = new Builder(1).Build(typeof(TestClass));
			Assert.IsNotNull(newClass);
            Assert.IsInstanceOf<TestClass>(newClass);
			var newClassTyped = (TestClass)newClass;
			Assert.IsNotNull(newClassTyped.Property1);
			Assert.IsNotNull(newClassTyped.Property2);
		}
		
		[Test]
		public void Builder_SeedReproducesTest_IntGenerator()
		{
			var newClass = new Builder(1).Build<int>();
			var newClass2 = new Builder(1).Build<int>();
			Assert.AreEqual(newClass, newClass2);
		}
		
		[Test]
		public void Builder_SeedReproducesTest_StringGenerator()
		{
			var newClass = new Builder(1).Build<string>();
			var newClass2 = new Builder(1).Build<string>();
			Assert.AreEqual(newClass, newClass2);
		}
		
		[Test]
		public void Build_SimpleClassMixesPrimitives()
		{
			var newClass = new Builder(1).Build<SimpleClass>();
			var newClass2 = new Builder(1).Build<SimpleClass>();
			
			Assert.IsNotNull(newClass.TestClass2Property);
			
			Assert.AreEqual(newClass.IntProperty, newClass2.IntProperty);
			Assert.AreEqual(newClass.StringProperty, newClass2.StringProperty);
		}
		
		[Test]
		public void Build_MaximumRecursionLevelSettable()
		{
			var newClass = new Builder(1).SetMaximumDepth(3).Build<RecursiveClass>();
			
			Assert.IsNotNull(newClass.RecursiveReference);
			Assert.IsNotNull(newClass.RecursiveReference.RecursiveReference);
			Assert.IsNotNull(newClass.RecursiveReference.RecursiveReference.RecursiveReference);
			Assert.IsNull(newClass.RecursiveReference.RecursiveReference.RecursiveReference.RecursiveReference);
						
		}

        public class RecursiveClass
        {
            public int IntProperty { get; set; }
            public RecursiveClass RecursiveReference { get; set; }
        }

        public class SimpleClass
        {
            public int IntProperty { get; set; }
            public string StringProperty { get; set; }
            public TestClass2 TestClass2Property { get; set; }
        }

        public class TestClass2
        {
        }

        public class TestClass
        {
            public int Property1 { get; set; }

            public TestClass2 Property2 { get; set; }
        }
	}
}
