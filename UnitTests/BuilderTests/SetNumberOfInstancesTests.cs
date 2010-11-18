
using System;
using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using NGineer.UnitTests.Types;

namespace NGineer.UnitTests.BuilderTests
{
	[TestFixture]
	public class SetNumberOfInstancesTests
	{
		[Test]
        public void SetNumberOfInstances_ObjectsProperlyReused()
        {
            var builder = new Builder(1)
                .SetNumberOfInstances<SimpleClass>(3, 3)
                .SetDefaultCollectionSize(10, 10)
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
        public void SetNumberOfInstances_ObjectsProperlyReused_InChildBuilder()
        {
            var builder = new Builder(1)
                .SetNumberOfInstances<SimpleClass>(2, 2)
                .For<SimpleClassParent>()
                    .Set(x => x.Child, (obj, buildr, session) => new []{
                        buildr.CreateNew().Build<SimpleClass>(),
                        buildr.CreateNew().Build<SimpleClass>(),
                        buildr.CreateNew().Build<SimpleClass>(),
                        buildr.CreateNew().Build<SimpleClass>(),
                        buildr.CreateNew().Build<SimpleClass>(),
                        buildr.CreateNew().Build<SimpleClass>()
                    })
                    .SetAfterBuild(x => x.UniqueChild, (obj, session) => session.ConstructedNodes.Select(e => e.Object).OfType<SimpleClass>().Distinct().ToArray())
                .Sealed();
            var parentClass = builder.Build<SimpleClassParent>();
            var simpleClassList = new List<SimpleClass>();
            int instances = 0;
            foreach (var simpleClass in parentClass.Child)
            {
                var existing = simpleClassList.FirstOrDefault(s => ReferenceEquals(s, simpleClass));
                if(existing == null)
                {
                    simpleClassList.Add(simpleClass);
                    instances++;
                }
            }
            Assert.AreEqual(2, instances);
            Assert.AreEqual(2, parentClass.UniqueChild.Length);
        }

        [Test]
        public void SetNumberOfInstances_ObjectsProperlyReused_ChildBuilder_WithinChildBuilder()
        {
            var builder = new Builder(1)
                .SetNumberOfInstances<SimpleClass>(2, 2)
                .CreateNew()
                .For<SimpleClassParent>()
                    .Set(x => x.Child, (obj, buildr, session) => new []{
                        buildr.CreateNew().Build<SimpleClass>(),
                        buildr.CreateNew().Build<SimpleClass>(),
                        buildr.CreateNew().Build<SimpleClass>(),
                        buildr.CreateNew().Build<SimpleClass>(),
                        buildr.CreateNew().Build<SimpleClass>(),
                        buildr.CreateNew().Build<SimpleClass>()
                    })
                    .SetAfterBuild(x => x.UniqueChild, (obj, session) => session.ConstructedNodes.Select(e => e.Object).OfType<SimpleClass>().Distinct().ToArray())
                .Sealed();
            var parentClass = builder.Build<SimpleClassParent>();
            var simpleClassList = new List<SimpleClass>();
            int instances = 0;
            foreach (var simpleClass in parentClass.Child)
            {
                var existing = simpleClassList.FirstOrDefault(s => ReferenceEquals(s, simpleClass));
                if(existing == null)
                {
                    simpleClassList.Add(simpleClass);
                    instances++;
                }
            }
            Assert.AreEqual(2, instances);
            Assert.AreEqual(2, parentClass.UniqueChild.Length);
        }

        [Test]
        public void SetNumberOfInstances_ObjectsNotRetouched_ButReprocessed_WhenReused()
        {
            var processCount = 0;
            var setCount = 0;
            var builder = new Builder(1)
                .For<SimpleClass>()
                    .Set(x => x.IntProperty, () => setCount++)
                    .Do(c => processCount++)
                .SetDefaultCollectionSize(5, 5)
                .SetNumberOfInstances<SimpleClass>(1, 1)
                .Sealed();
            var instances = builder.Build<SimpleClass[]>();

            Assert.AreSame(instances[0], instances[1]);
            Assert.AreEqual(1, setCount);
            Assert.AreEqual(5, processCount);
        }

        [Test]
        public void SetNumberOfInstances_ObjectsProperlyReused_EvenIfInstanceIsReusedDuringInitialization()
        {
            var newClass = new Builder()
                .SetNumberOfInstances<CountsPropertySets>(1, 1)
                .For<CountsPropertySets>()
                    .Set(x => x.RecursiveProperty, (o, b, s) => b.Build<CountsPropertySets>())
                    .Set(x => x.SomeProperty, () => 10)
                .Build<CountsPropertySets>();

            Assert.AreEqual(newClass, newClass.RecursiveProperty);
            Assert.AreEqual(1, newClass.GetSomePropertySets());
            Assert.AreEqual(1, newClass.GetRecursivePropertySets());

            Assert.IsNotNull(newClass.SomeProperty);
        }
	}
}
