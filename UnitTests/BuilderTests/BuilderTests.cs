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
		public void Build_MaximumRecursionLevel_Settable()
		{
			var newClass = new Builder(1).SetMaximumDepth(3).Build<RecursiveClass>();
			
			Assert.IsNotNull(newClass.RecursiveReference);
			Assert.IsNotNull(newClass.RecursiveReference.RecursiveReference);
			Assert.IsNotNull(newClass.RecursiveReference.RecursiveReference.RecursiveReference);
			Assert.IsNull(newClass.RecursiveReference.RecursiveReference.RecursiveReference.RecursiveReference);
		}

        [Test]
        public void Build_MaximumRecursionLevel_SetByDefault_ThrowsExceptionWhenReached()
        {
            Assert.Throws<BuilderDepthExceededException>(() => new Builder(1).Build<RecursiveClass>());
        }

        [Test]
        public void Build_MaximumRecursionLevel_ManuallySet_CanThrowExceptionWhenReached()
        {
            Assert.Throws<BuilderDepthExceededException>(() => new Builder(1)
                .SetMaximumDepth(10)
                .ThrowsWhenMaximumDepthReached()
                .Build<RecursiveClass>());
        }

		[Test]
		public void Build_MaximumRecursionLevel_ChildContainer_SetValueBuilderEnforced()
		{
		    var sessions = new List<BuildSession>();
			var newClass = new Builder(1)
				.SetMaximumDepth(2)
				.AfterPopulationOf<TestClassFourDeep>((t, b, s) => {
                    sessions.Add(s);
					t.PropertyTestClass = b.CreateNew()
						.AfterPopulationOf<TestClassThreeDeep>((t1, b1, s1) => {
                            sessions.Add(s1);
							t1.PropertyTestClass = b1.CreateNew()
								.AfterPopulationOf<TestClass>((t2, b2, s2) => {
                                    sessions.Add(s2);
									t2.Property2 = b2.CreateNew().Build<TestClass2>(s2);
								})
								.Build<TestClass>(s1);
						})
						.Build<TestClassThreeDeep>(s);
				})
				.Build<TestClassFourDeep>();

		    Assert.IsNotNull(newClass.PropertyTestClass);
			Assert.IsNotNull(newClass.PropertyTestClass.PropertyTestClass);
			Assert.IsNull(newClass.PropertyTestClass.PropertyTestClass.Property2);
		}
		
		[Test]
		public void Build_MaximumRecursionLevel_SetValueBuilderEnforced()
		{
			List<int> buildDepth = new List<int>();
			var newClass = new Builder(1)
				.SetMaximumDepth(2)
				.AfterPopulationOf<TestClassFourDeep>((type, b, s) => {
					buildDepth.Add(s.BuildDepth);
					type.PropertyTestClass = b
                        .CreateNew()
						.AfterPopulationOf<TestClassThreeDeep>((type1, b1, s1) => {
							buildDepth.Add(s1.BuildDepth);
							type1.PropertyTestClass = b1
                                .CreateNew()
								.AfterPopulationOf<TestClass>((type2, b2, s2) => {
									buildDepth.Add(s2.BuildDepth);
									type2.Property2 = b2.Build<TestClass2>(s2);
								})
								.Build<TestClass>(s1);
						})
						.Build<TestClassThreeDeep>(s);
				})
				.Build<TestClassFourDeep>();
			
			Assert.AreEqual(new int[]{0,1,2}, buildDepth.ToArray());
			Assert.IsNotNull(newClass.PropertyTestClass);
			Assert.IsNotNull(newClass.PropertyTestClass.PropertyTestClass);
			Assert.IsNull(newClass.PropertyTestClass.PropertyTestClass.Property2);
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
			Assert.Throws<BuilderException>(() => newClass.SetCollectionSize(10, 100));
            Assert.Throws<BuilderException>(() => newClass.AfterPopulationOf<int>(n => { }));
            Assert.Throws<BuilderException>(() => newClass.WithGenerator(null));
        }

        [Test]
        public void Sealed_BuildSealsBuilder()
        {
            var builder = new Builder(1).AfterPopulationOf<int>(n => { });
            builder.Build<int>();
            Assert.Throws<BuilderException>(() => builder.SetMaximumDepth(10));
            Assert.Throws<BuilderException>(() => builder.SetCollectionSize(10, 100));
            Assert.Throws<BuilderException>(() => builder.AfterPopulationOf<int>(n => { }));
            Assert.Throws<BuilderException>(() => builder.WithGenerator(null));
        }

        [Test]
        public void Sealed_SealOnChildSealsParent()
        {
            var builder = new Builder(1).AfterPopulationOf<int>(n => { });
            builder.CreateNew().Sealed();
            Assert.Throws<BuilderException>(() => builder.SetMaximumDepth(10));
            Assert.Throws<BuilderException>(() => builder.SetCollectionSize(10, 100));
            Assert.Throws<BuilderException>(() => builder.AfterPopulationOf<int>(n => { }));
            Assert.Throws<BuilderException>(() => builder.WithGenerator(null));
        }

		[Test]
        public void CollectionSize_DefaultSettable()
        {
            var newClass = new Builder(1)
                .SetCollectionSize(50, 60)
                .Build<IList<SimpleClass>>();

			Assert.IsNotNull(newClass);
            Assert.IsTrue(50 <= newClass.Count && newClass.Count <= 60);
        }

        [Test]
        public void CollectionSize_UseParentDefault_BeforeClassDefault()
        {
            var newClass = new Builder(1)
                .SetCollectionSize(50, 50)
                .CreateNew()
                .Build<IList<SimpleClass>>();

            Assert.AreEqual(50, newClass.Count);
        }

        [Test]
        public void CollectionSize_SettablePerType()
        {
            var newClass = new Builder(1)
                .SetCollectionSize(50, 50)
                .SetCollectionSize<SimpleClass>(10, 10)
                .Build<IList<SimpleClass>>();

            Assert.IsNotNull(newClass);
            Assert.AreEqual(10, newClass.Count);
        }

        [Test]
        public void CollectionSize_SettablePerType_ChildInherits()
        {
            var newClass = new Builder(1)
                .SetCollectionSize(50, 50)
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
                .SetCollectionSize(50, 50)
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
        public void SetNumberOfInstances_ObjectsProperlyReused()
        {
            var builder = new Builder(1)
                .SetNumberOfInstances<SimpleClass>(3, 3)
                .SetCollectionSize(10, 10)
                .Sealed();
            var simpleClassArray = builder.Build<SimpleClass[]>();
            var simpleClassList = new List<SimpleClass>();
            int instances = 0;
            foreach (var simpleClass in simpleClassArray)
            {
                var existing = simpleClassList.FirstOrDefault(s => ReferenceEquals(s, simpleClass));
                if(existing == null)
                {
                    simpleClassList.Add(simpleClass);
                    instances++;
                }
            }
            Assert.AreEqual(3, instances);
        }

        [Test]
        public void SetNumberOfInstances_ObjectsNotRetouched_WhenReused()
        {
            var count = 0;
            var builder = new Builder(1)
                .AfterPopulationOf<SimpleClass>(c => count++)
                .SetCollectionSize(5,5)
                .SetNumberOfInstances<SimpleClass>(1, 1)
                .Sealed();
            var instances = builder.Build<SimpleClass[]>();

            Assert.AreSame(instances[0], instances[1]);
            Assert.AreEqual(1, count);
        }
	
		[Test]
		public void Hierarchy_BuildDepthInheritedFromParent()
		{
			var builder = new Builder(1);
			var builder2 = builder.CreateNew();
			builder.SetMaximumDepth(3);
			Assert.AreEqual(builder.BuildDepth, builder2.BuildDepth);
		}
		
		[Test]
		public void Hierarchy_ChildCanBeOverridden()
		{
			var builder = new Builder(1);
			var builder2 = builder.CreateNew();
			builder.SetMaximumDepth(3);
			builder2.SetMaximumDepth(5);
			
			Assert.AreNotEqual(builder.BuildDepth, builder2.BuildDepth);
			Assert.AreEqual(3, builder.BuildDepth);
			Assert.AreEqual(5, builder2.BuildDepth);
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
