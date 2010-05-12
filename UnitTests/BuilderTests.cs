using System;
using System.Linq;
using NGineer.BuildHelpers;
using NGineer.Generators;
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
		public void Build_MaximumRecursionLevel_Settable()
		{
			var newClass = new Builder(1).SetMaximumDepth(3).Build<RecursiveClass>();
			
			Assert.IsNotNull(newClass.RecursiveReference);
			Assert.IsNotNull(newClass.RecursiveReference.RecursiveReference);
			Assert.IsNotNull(newClass.RecursiveReference.RecursiveReference.RecursiveReference);
			Assert.IsNull(newClass.RecursiveReference.RecursiveReference.RecursiveReference.RecursiveReference);
		}

        [Test]
        [MaxTime(5000)]
        public void Build_MaximumRecursionLevel_SetByDefault()
        {
            new Builder(1).Build<RecursiveClass>();
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
									return t2;
								})
								.Build<TestClass>(s1);
							return t1;	
						})
						.Build<TestClassThreeDeep>(s);
					return t;	
				})
				.Build<TestClassFourDeep>();

		    var session = sessions.First();
		    foreach (var buildSession in sessions)
		    {
                Assert.AreSame(session, buildSession);
		    }
			Assert.IsNotNull(newClass.PropertyTestClass);
			Assert.IsNotNull(newClass.PropertyTestClass.PropertyTestClass);
			Assert.IsNull(newClass.PropertyTestClass.PropertyTestClass.Property2);
		}
		
		[Test]
		public void Build_MaximumRecursionLevel_SetValueBuilderEnforced()
		{
			List<BuildSession> sessions = new List<BuildSession>();
			List<int> buildDepth = new List<int>();
			var newClass = new Builder(1)
				.SetMaximumDepth(2)
				.AfterPopulationOf<TestClassFourDeep>((type, b, s) => {
					sessions.Add(s);
					buildDepth.Add(s.BuildDepth);
					type.PropertyTestClass = b
                        .CreateNew()
						.AfterPopulationOf<TestClassThreeDeep>((type1, b1, s1) => {
							sessions.Add(s1);
							buildDepth.Add(s1.BuildDepth);
							type1.PropertyTestClass = b1
                                .CreateNew()
								.AfterPopulationOf<TestClass>((type2, b2, s2) => {
									sessions.Add(s2);
									buildDepth.Add(s2.BuildDepth);
									type2.Property2 = b2.Build<TestClass2>(s2);
									return type2;
								})
								.Build<TestClass>(s1);
							return type1;	
						})
						.Build<TestClassThreeDeep>(s);
					return type;	
				})
				.Build<TestClassFourDeep>();
			
			Assert.AreEqual(new int[]{1,2,3}, buildDepth.ToArray());
			var first = sessions.First();
			foreach(var session in sessions)
			{
				Assert.AreSame(first, session);	
			}
			Assert.IsNotNull(newClass.PropertyTestClass);
			Assert.IsNotNull(newClass.PropertyTestClass.PropertyTestClass);
			Assert.IsNull(newClass.PropertyTestClass.PropertyTestClass.Property2);
		}
		
		[Test]
		public void Build_Setters_ChildSettersRunLast()
		{
			var newClass = new Builder(1)
				.SetMaximumDepth(2)
				.AfterPopulationOf<SimpleClass>(s => s.IntProperty = 10)
				.CreateNew()
				.AfterPopulationOf<SimpleClass>(s => s.IntProperty = 30)
				.Build<SimpleClass>();
			
			Assert.AreEqual(30, newClass.IntProperty);
		}
		
		[Test]
		public void Build_SetValues_CantModifyWithinCall()
		{
			var newClass = new Builder(1)
				.SetMaximumDepth(2)
				.AfterPopulationOf<RecursiveClass>((t, b, s) => {
					t.RecursiveReference = b
							.AfterPopulationOf<RecursiveClass>(s1 => s1)
							.Build<RecursiveClass>(s);
					return t;	
				});
			Assert.Throws<BuilderException>(() => newClass.Build<RecursiveClass>());
		}

        [Test]
        public void Build_SettersAreProperlyCalled_SimpleInt()
        {
            var newClass = new Builder(1).AfterPopulationOf<int>(n => 190).Build<int>();
            Assert.AreEqual(190, newClass);
        }

        [Test]
        public void Build_SettersAreProperlyCalled_SimpleClass()
        {
            var newClass = new Builder(1).AfterPopulationOf<int>(n => 190).Build<SimpleClass>();
            Assert.AreEqual(190, newClass.IntProperty);
            Assert.IsNotNull(newClass.StringProperty);
            Assert.IsNotNull(newClass.TestClass2Property);
        }

        [Test]
        public void Sealed_SealedBuilderPreventsModification()
        {
            var newClass = new Builder(1).AfterPopulationOf<int>(n => 190).Sealed();
            Assert.Throws<BuilderException>(() => newClass.SetMaximumDepth(10));
			Assert.Throws<BuilderException>(() => newClass.SetCollectionSize(10, 100));
            Assert.Throws<BuilderException>(() => newClass.AfterPopulationOf<int>(n => 10));
            Assert.Throws<BuilderException>(() => newClass.WithGenerator(null));
        }

        [Test]
        public void Build_SetupValueToOverrideBehaviour_SimpleClass()
        {
            var newClass = new Builder(1)
                .AfterPopulationOf<string>((t, b, s) => b.Build(typeof(string), s))
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
                .AfterPopulationOf<int>(n => 10)
                .AfterPopulationOf<RecursiveClass>((t, b, s) =>
                                                  {
                                                      t.RecursiveReference = b.CreateNew()
                                                          .AfterPopulationOf<int>(c => 20)
                                                          .Build<RecursiveClass>(s);
                                                      return t;
                                                  })
                .Sealed()
                .Build<RecursiveClass>();

            Assert.IsNotNull(newClass.RecursiveReference);
            Assert.AreEqual(10, newClass.IntProperty);
            Assert.AreEqual(20, newClass.RecursiveReference.IntProperty);
        }

        [Test]
        public void Build_AfterConstructionOf_PropertiesOnlySetOnce()
        {
            var newClass = new Builder(1)
                .AfterConstructionOf<CountsPropertySets>(c => c.RecursiveProperty, (o, b, s) => null)
                .Build<CountsPropertySets>();

            Assert.AreEqual(1, newClass.GetSomePropertySets());
            Assert.AreEqual(1, newClass.GetRecursivePropertySets());

            Assert.IsNotNull(newClass.SomeProperty);
            Assert.IsNull(newClass.RecursiveProperty);
        }

        [Test]
        public void Build_AfterConstructionOf_NullableIntProperty()
        {
            IBuilder builder = null;
            Assert.DoesNotThrow(() => builder = new Builder(1)
                .AfterConstructionOf<ClassWithNullableInt>(c => c.Property1, 9));

            var obj = builder.Build<ClassWithNullableInt>();
            Assert.AreEqual(9, obj.Property1.Value);
        }

        [Test]
        public void Build_AfterConstructionOf_NullableDateTimeProperty()
        {
            var dateTime = DateTime.Now;
            IBuilder builder = null;
            Assert.DoesNotThrow(() => builder = new Builder(1)
                .AfterConstructionOf<ClassWithNullableDateTime>(c => c.Property1, dateTime));

            var obj = builder.Build<ClassWithNullableDateTime>();
            Assert.AreEqual(dateTime, obj.Property1.Value);
        }

        [Test]
        public void Build_AfterConstructionOf_Inherited_NullableDateTimeProperty()
        {
            var dateTime = DateTime.Now;
            IBuilder builder = null;
            Assert.DoesNotThrow(() => builder = new Builder(1)
                .AfterConstructionOf<InheritsFromClassWithNullableDateTime>(c => c.Property1, dateTime));

            var obj = builder.Build<InheritsFromClassWithNullableDateTime>();
            Assert.AreEqual(dateTime, obj.Property1.Value);
        }

        [Test]
        public void Build_AfterConstructionOf_ObjectAddedToStackAfterConstruction()
        {
            object constructedInstance = null;
            new Builder(1)
                .AfterConstructionOf<SimpleClass>(c => c.StringProperty, 
                (o, b, s) =>
                {
                    constructedInstance = s.ConstructedObjects.FirstOrDefault(c => c is SimpleClass);
                    return null;
                })
                .Build<SimpleClass>();

            Assert.IsNotNull(constructedInstance);
        }

        [Test]
        public void Build_SetMaximumInstances_ObjectsProperlyReused()
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

        [Test]
        public void Hierarchy_ChildCallsParent_PopulatorOf()
        {
            var parentCalled = -1;
            var childCalled = -1;
            var callOrder = 0;
            var builder = new Builder(1).AfterPopulationOf<SimpleClass>((o, b, s) =>
                {
                    parentCalled = callOrder++;
                    o.IntProperty = 10;
                });
            var newClass = builder.CreateNew()
                .AfterPopulationOf<SimpleClass>((o, b, s) =>
                {
                    childCalled = callOrder++;
                    o.IntProperty = 11;
                })
                .Build<SimpleClass>();

            Assert.AreNotEqual(-1, parentCalled, "Parent wasn't called");
            Assert.AreNotEqual(-1, childCalled, "Child wasn't called");
            Assert.IsTrue(childCalled > parentCalled, "Child called before parent");
            Assert.AreEqual(11, newClass.IntProperty);
        }

        [Test]
        public void Hierarchy_ChildCallsParentFirst_ConstructionOf()
        {
            var parentCalled = -1;
            var childCalled = -1;
            var callOrder = 0;
            var builder = new Builder(1).AfterConstructionOf<SimpleClass>(c => c.IntProperty, 
                (o, b, s) => {
                    parentCalled = callOrder++;
                    return 10;
            });
            var newClass = builder.CreateNew()
                .AfterConstructionOf<SimpleClass>(c => c.IntProperty,
                (o, b, s) =>
                {
                    childCalled = callOrder++;
                    return 11;
                })
                .Build<SimpleClass>();

            Assert.AreNotEqual(-1, parentCalled, "Parent wasn't called");
            Assert.AreNotEqual(-1, childCalled, "Child wasn't called");
            Assert.IsTrue(childCalled > parentCalled, "Child called before parent");
            Assert.AreEqual(11, newClass.IntProperty);
        }

        public class CountsPropertySets
        {
            private int _somePropertySets;
            public int GetSomePropertySets()
            {
                return _somePropertySets;
            }

            private int _someProperty;
            public int SomeProperty
            {
                get { return _someProperty; }
                set
                {
                    _someProperty = value;
                    _somePropertySets++;
                }
            }

            private int _recursivePropertySets;
            public int GetRecursivePropertySets()
            {
                return _recursivePropertySets;
            }

            private CountsPropertySets _recursiveProperty;
            public CountsPropertySets RecursiveProperty
            {
                get { return _recursiveProperty; }
                set
                {
                    _recursiveProperty = value;
                    _recursivePropertySets++;
                }
            }
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

        public class ClassWithNullableInt
        {
            public int? Property1 { get; set; }
        }

        public class ClassWithNullableDateTime
        {
            public DateTime? Property1 { get; set; }
        }

        public class InheritsFromClassWithNullableDateTime : ClassWithNullableDateTime
        {
        }

        public class TestClass
        {
            public int Property1 { get; set; }

            public TestClass2 Property2 { get; set; }
        }
		
		public class TestClassThreeDeep
		{
			public TestClass PropertyTestClass { get; set; }
		}
		
		public class TestClassFourDeep
		{
			public TestClassThreeDeep PropertyTestClass { get; set; }
		}
	}
}
