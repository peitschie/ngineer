using System;
using System.Linq;
using NGineer.BuildHelpers;
using NUnit.Framework;
using System.Collections.Generic;


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

        [Test]
        public void Build_SettersAreProperlyCalled_SimpleInt()
        {
            var newClass = new Builder(1).SetValuesFor<int>(n => 190).Build<int>();
            Assert.AreEqual(190, newClass);
        }

        [Test]
        public void Build_SettersAreProperlyCalled_SimpleClass()
        {
            var newClass = new Builder(1).SetValuesFor<int>(n => 190).Build<SimpleClass>();
            Assert.AreEqual(190, newClass.IntProperty);
            Assert.IsNotNull(newClass.StringProperty);
            Assert.IsNotNull(newClass.TestClass2Property);
        }

        [Test]
        public void Seal_SealedBuilderPreventsModification()
        {
            var newClass = new Builder(1).SetValuesFor<int>(n => 190).Seal();
            Assert.Throws<BuilderException>(() => newClass.SetMaximumDepth(10));
			Assert.Throws<BuilderException>(() => newClass.SetCollectionSize(10, 100));
            Assert.Throws<BuilderException>(() => newClass.SetValuesFor<int>(n => 10));
            Assert.Throws<BuilderException>(() => newClass.WithGenerator(null));
        }

        [Test]
        public void Build_SetupValueToOverrideBehaviour_SimpleClass()
        {
            var newClass = new Builder(1)
                .SetValuesFor<string>((s, b) => b.Build(typeof(string)))
                .Build<SimpleClass>();

            Assert.IsNotNull(newClass.StringProperty);
        }
		
		[Test]
        public void Build_SetCollectionSize_Settable()
        {
            var newClass = new Builder(1)
                .SetCollectionSize(50, 60)
                .Build<IList<SimpleClass>>();

			Assert.IsNotNull(newClass);
            Assert.IsTrue(50 <= newClass.Count && newClass.Count <= 60);
        }

        [Test]
        public void Build_SetupValueToOverrideBehaviour_RecursiveClass()
        {
            var newClass = new Builder(1)
                .SetValuesFor<int>(n => 10)
                .SetValuesFor<RecursiveClass>((s, b) =>
                                                  {
                                                      s.RecursiveReference = b.CreateNew()
                                                          .SetValuesFor<int>(c => 20)
                                                          .Build<RecursiveClass>();
                                                      return s;
                                                  })
                .Seal()
                .Build<RecursiveClass>();

            Assert.IsNotNull(newClass.RecursiveReference);
            Assert.AreEqual(10, newClass.IntProperty);
            Assert.AreEqual(20, newClass.RecursiveReference.IntProperty);
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
