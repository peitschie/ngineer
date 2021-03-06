using System;
using System.Collections;
using System.Linq;
using NGineer.Internal;
using NGineer.Utils;
using NUnit.Framework;
using System.Collections.Generic;
using System.Reflection;
using NGineer.Exceptions;
using NGineer.UnitTests.Types;


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
        public void Builder_BuilderOrderIsConsistent()
        {
            var sequence = 0;
            var newClass = new Builder(1)
                .WithGenerator(() => sequence++)
                .Build<BuilderOrderLevel3>();
                
            Assert.IsNotNull(newClass);
            sequence = 0;
            Assert.AreEqual(new []{0,1,2,3,4,5,6,7}, 
                new []{newClass.Ap, newClass.Bp, newClass.Cp, newClass.Dp,
                newClass.A, newClass.B, newClass.C, newClass.D});
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
				.For<RecursiveClass>().Do((t, b, s) => {
					t.RecursiveReference = b
							.For<RecursiveClass>().Do(s1 => {})
							.Build<RecursiveClass>();
				});
			var wrappedException = Assert.Throws<WrappedBuilderException>(() => newClass.Build<RecursiveClass>());
            Assert.IsInstanceOf<BuilderSealedException>(wrappedException.InnerException);
		}

        [Test]
        public void Sealed_SealedBuilderPreventsModification()
        {
            var newClass = new Builder(1).For<int>().Do(n => { }).Sealed();
            Assert.Throws<BuilderSealedException>(() => newClass.SetMaximumDepth(10));
            Assert.Throws<BuilderSealedException>(() => newClass.SetDefaultCollectionSize(10, 100));
            Assert.Throws<BuilderSealedException>(() => newClass.For<int>().Do(n => { }));
            Assert.Throws<BuilderSealedException>(() => newClass.WithGenerator(null));
        }

        [Test]
        public void Sealed_BuildSealsBuilder()
        {
            var builder = new Builder(1).For<int>().Do(n => { });
            builder.Build<int>();
            Assert.Throws<BuilderSealedException>(() => builder.SetMaximumDepth(10));
            Assert.Throws<BuilderSealedException>(() => builder.SetDefaultCollectionSize(10, 100));
            Assert.Throws<BuilderSealedException>(() => builder.For<int>().Do(n => { }));
            Assert.Throws<BuilderSealedException>(() => builder.WithGenerator(null));
        }

        [Test]
        public void Sealed_SealOnChildSealsParent()
        {
            var builder = new Builder(1).For<int>().Do(n => { });
            builder.CreateNew().Sealed();
            Assert.Throws<BuilderSealedException>(() => builder.SetMaximumDepth(10));
            Assert.Throws<BuilderSealedException>(() => builder.SetDefaultCollectionSize(10, 100));
            Assert.Throws<BuilderSealedException>(() => builder.For<int>().Do(n => { }));
            Assert.Throws<BuilderSealedException>(() => builder.WithGenerator(null));
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
				.For<int[][]>().Do((o, b, s) => {
						for(int i = 0; i < o.Length; i++)
						{
							o[i] = b.CreateNew()
									.SetCollectionSize<int>(20,20)
									.Build<int[]>();
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
                .For<RecursiveClass>()
					.Do(n => n.IntProperty = 10)
                	.Do((t, b, s) =>
                                                  {
                                                      t.RecursiveReference = b.CreateNew()
                                                          .For<RecursiveClass>().Do(c => c.IntProperty = 20)
                                                          .Build<RecursiveClass>();
                                                  })
                .Sealed()
                .Build<RecursiveClass>();

            Assert.IsNotNull(newClass.RecursiveReference);
            Assert.AreEqual(10, newClass.IntProperty);
            Assert.AreEqual(20, newClass.RecursiveReference.IntProperty);
        }

        [Test]
        public void Build_DepthReached_ReturnsDefaultValueForType()
        {
            var result = new Builder().SetMaximumDepth(0)
                .For<SimpleClass>()
                    .Set(x => x.IntProperty, (o, b, s) => b.Build<int>())
                .Build<SimpleClass>();
            Assert.AreEqual(0, result.IntProperty);
        }

        [Test]
        public void Hierarchy_ChildDoesntBuildIdenticalObjectAsParent()
        {
            var builder = new Builder(1);
            var builder2 = builder.CreateNew();

            Assert.AreNotEqual(builder.Build<int>(), builder2.Build<int>());
        }

        [Test]
        public void Build_ObjectIsLeftAsNull()
        {
            var result = new Builder().Build<ClassWithObject>();
            Assert.IsNull(result.ObjectMember);
            Assert.IsNotNull(result.NormalMember);
        }
    }
}
