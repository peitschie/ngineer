
using System;
using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;

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
        public void SetNumberOfInstances_ObjectsNotRetouched_WhenReused()
        {
            var count = 0;
            var builder = new Builder(1)
                .For<SimpleClass>().Do(c => count++)
                .SetDefaultCollectionSize(5,5)
                .SetNumberOfInstances<SimpleClass>(1, 1)
                .Sealed();
            var instances = builder.Build<SimpleClass[]>();

            Assert.AreSame(instances[0], instances[1]);
            Assert.AreEqual(1, count);
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
