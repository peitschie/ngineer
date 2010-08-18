using System;
using System.Collections;
using System.Linq;
using NGineer.BuildHelpers;
using NGineer.Utils;
using NUnit.Framework;
using System.Collections.Generic;
using System.Reflection;


namespace NGineer.UnitTests.BuilderTests
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
		public void Build_SetValues_CantModifyWithinCall()
		{
			var newClass = new Builder(1)
				.SetMaximumDepth(2)
				.AfterPopulationOf<RecursiveClass>((t, b, s) => {
					t.RecursiveReference = b
							.AfterPopulationOf<RecursiveClass>(s1 => {})
							.Build<RecursiveClass>(s);
				});
			Assert.Throws<BuilderException>(() => newClass.Build<RecursiveClass>());
		}

        [Test]
        public void Sealed_SealedBuilderPreventsModification()
        {
            var newClass = new Builder(1).AfterPopulationOf<int>(n => { }).Sealed();
            Assert.Throws<BuilderException>(() => newClass.SetMaximumDepth(10));
			Assert.Throws<BuilderException>(() => newClass.SetDefaultCollectionSize(10, 100));
            Assert.Throws<BuilderException>(() => newClass.AfterPopulationOf<int>(n => { }));
            Assert.Throws<BuilderException>(() => newClass.WithGenerator(null));
        }

        [Test]
        public void Sealed_BuildSealsBuilder()
        {
            var builder = new Builder(1).AfterPopulationOf<int>(n => { });
            builder.Build<int>();
            Assert.Throws<BuilderException>(() => builder.SetMaximumDepth(10));
            Assert.Throws<BuilderException>(() => builder.SetDefaultCollectionSize(10, 100));
            Assert.Throws<BuilderException>(() => builder.AfterPopulationOf<int>(n => { }));
            Assert.Throws<BuilderException>(() => builder.WithGenerator(null));
        }

        [Test]
        public void Sealed_SealOnChildSealsParent()
        {
            var builder = new Builder(1).AfterPopulationOf<int>(n => { });
            builder.CreateNew().Sealed();
            Assert.Throws<BuilderException>(() => builder.SetMaximumDepth(10));
            Assert.Throws<BuilderException>(() => builder.SetDefaultCollectionSize(10, 100));
            Assert.Throws<BuilderException>(() => builder.AfterPopulationOf<int>(n => { }));
            Assert.Throws<BuilderException>(() => builder.WithGenerator(null));
        }

		[Test]
        public void CollectionSize_DefaultSettable()
        {
            var newClass = new Builder(1)
                .SetDefaultCollectionSize(50, 60)
                .Build<IList<SimpleClass>>();

			Assert.IsNotNull(newClass);
            Assert.IsTrue(50 <= newClass.Count && newClass.Count <= 60);
        }

        [Test]
        public void CollectionSize_UseParentDefault_BeforeClassDefault()
        {
            var newClass = new Builder(1)
                .SetDefaultCollectionSize(50, 50)
                .CreateNew()
                .Build<IList<SimpleClass>>();

            Assert.AreEqual(50, newClass.Count);
        }

        [Test]
        public void CollectionSize_SettablePerType()
        {
            var newClass = new Builder(1)
                .SetDefaultCollectionSize(50, 50)
                .SetCollectionSize<SimpleClass>(10, 10)
                .Build<IList<SimpleClass>>();

            Assert.IsNotNull(newClass);
            Assert.AreEqual(10, newClass.Count);
        }

        [Test]
        public void CollectionSize_SettablePerType_ChildInherits()
        {
            var newClass = new Builder(1)
                .SetDefaultCollectionSize(50, 50)
                .SetCollectionSize<SimpleClass>(10, 10)
                .CreateNew()
                .Build<IList<SimpleClass>>();

            Assert.IsNotNull(newClass);
            Assert.AreEqual(10, newClass.Count);
        }

        [Test]
        public void CollectionSize_SettablePerType_CanOverride_WithinPopulator_ReuseSession()
        {
            var newClass = new Builder(1)
                .SetDefaultCollectionSize(50, 50)
                .SetCollectionSize<int>(10, 10)
				.SetCollectionSize<int[]>(30, 30)
				.AfterPopulationOf<int[][]>((o, b, s) => {
						for(int i = 0; i < o.Length; i++)
						{
							o[i] = b.CreateNew()
									.SetCollectionSize<int>(20,20)
									.Build<int[]>(s);
						}
					})
                .Build<int[][]>();

            Assert.IsNotNull(newClass);
            Assert.AreEqual(30, newClass.Length);
			Assert.AreEqual(20, newClass[0].Length);
        }
		
		
        [Test]
        public void Build_SetupValueToOverrideBehaviour_RecursiveClass()
        {
            var newClass = new Builder(1)
                .SetMaximumDepth(3)
                .AfterPopulationOf<RecursiveClass>(n => n.IntProperty = 10)
                .AfterPopulationOf<RecursiveClass>((t, b, s) =>
                                                  {
                                                      t.RecursiveReference = b.CreateNew()
                                                          .AfterPopulationOf<RecursiveClass>(c => c.IntProperty = 20)
                                                          .Build<RecursiveClass>(s);
                                                  })
                .Sealed()
                .Build<RecursiveClass>();

            Assert.IsNotNull(newClass.RecursiveReference);
            Assert.AreEqual(10, newClass.IntProperty);
            Assert.AreEqual(20, newClass.RecursiveReference.IntProperty);
        }

        [Test]
        public void Hierarchy_ChildDoesntBuildIdenticalObjectAsParent()
        {
            var builder = new Builder(1);
            var builder2 = builder.CreateNew();

            Assert.AreNotEqual(builder.Build<int>(), builder2.Build<int>());
        }
    }
}
