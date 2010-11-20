using NGineer.UnitTests.Types;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace NGineer.UnitTests.Examples
{
    [TestFixture]
    public class Recipies
    {
        private IBuilder _builder;

        [SetUp]
        public void SetUp()
        {
            _builder = new Builder();
        }

        #region BuildArrayOfKnownSize
        [Test]
        public void BuildArrayOfKnownSize()
        {
            var array = _builder
                .SetCollectionSize<string>(10, 20)
                .Build<string[]>();

            Assert.That(array.Length, Is.GreaterThanOrEqualTo(10));
            Assert.That(array.Length, Is.LessThanOrEqualTo(20));
        }
        #endregion

        #region Build_ReusingExistingInstancesOnly
        public class ReuseInstance { public string Name; }
        public class ParentInstance { public ReuseInstance Instance; }

        [Test]
        public void Build_ReusingExistingInstancesOnly()
        {
            var existingEntity1 = new ReuseInstance {Name = "ExistingInstance1"};
            var existingEntity2 = new ReuseInstance {Name = "ExistingInstance2"};
            var existingEntity3 = new ReuseInstance {Name = "ExistingInstance3"};

            var instances = _builder
                .WithGenerator(Generators.ReuseOnly(existingEntity1, existingEntity2, existingEntity3))
                .Build<ParentInstance[]>();

            foreach (var instance in instances)
            {
                Assert.IsTrue(ReferenceEquals(instance.Instance, existingEntity1) 
                    || ReferenceEquals(instance.Instance, existingEntity2) 
                    || ReferenceEquals(instance.Instance, existingEntity3));
            }
        }
        #endregion

        #region Build_GetAllConstructedInstances
        public class StringLists
        {
            public IList<string> List1;
            public string[] List2;
            public IEnumerable<string> List3;
            public IList<string> AllInstances;
        }

        [Test]
        public void Build_GetAllConstructedInstances()
        {
            var instances = _builder
                .For<StringLists>()
                    .SetAfterBuild(x => x.AllInstances, (builder, session) => session.ConstructedNodes
                                                                .Select(o => o.Object)
                                                                .OfType<string>().ToList())
               .Build<StringLists>();

            foreach (var list in instances.List1)
            {
                Assert.IsTrue(instances.AllInstances.Contains(list));
            }
            foreach (var list in instances.List2)
            {
                Assert.IsTrue(instances.AllInstances.Contains(list));
            }
            foreach (var list in instances.List3)
            {
                Assert.IsTrue(instances.AllInstances.Contains(list));
            }
        }
        #endregion
    }
}