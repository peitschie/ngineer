using NGineer.BuildHelpers;
using NUnit.Framework;

namespace NGineer.UnitTests.BuildHelpers
{
    [TestFixture]
    public class BuildSessionTests
    {
        private BuildSession _session;

        [Test]
        public void GetCollectionSize_ReturnsDefaultIfNoneDefined()
        {
            var defRange = new Range(10, 1000);
            _session = new BuildSession(new TypeRegistry<Range>(), defRange);
            Assert.AreSame(defRange, _session.GetCollectionSize(typeof(string)));
        }

        [Test]
        public void GetCollectionSize_ReturnsDefinedRange()
        {
            var registry = new TypeRegistry<Range>();
            var stringRange = new Range(-10, 1); 
            var defRange = new Range(10, 1000);
            registry.SetForType<string>(stringRange);
            _session = new BuildSession(registry, defRange);
            
            Assert.AreSame(stringRange, _session.GetCollectionSize(typeof(string)));
        }
    }
}