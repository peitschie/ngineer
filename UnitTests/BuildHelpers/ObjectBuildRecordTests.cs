using System.Linq;
using NGineer.BuildHelpers;
using NUnit.Framework;

namespace NGineer.UnitTests.BuildHelpers
{
    [TestFixture]
    public class ObjectBuildRecordTests
    {
        private ObjectBuildRecord _buildRecord;

        [Test]
        public void Constructor_UsesObjectTypeForRetrievingProperties()
        {
            _buildRecord = new ObjectBuildRecord(typeof(SimpleClassBase), new SimpleClass());

            var propertyNames = _buildRecord.UnconstructedMembers.Select(p => p.Name).OrderBy(n => n).ToArray();
            Assert.AreEqual(new []{"Property1", "Property2"}, propertyNames);
        }

        [Test]
        public void Constructor_NullObject()
        {
            Assert.DoesNotThrow(() => _buildRecord = new ObjectBuildRecord(typeof(SimpleClassBase), null));

            var propertyNames = _buildRecord.UnconstructedMembers.Select(p => p.Name).OrderBy(n => n).ToArray();
            Assert.AreEqual(new []{"Property1"}, propertyNames);
        }

        private class SimpleClassBase
        {
            public int Property1 { get; set; }
        }

        private class SimpleClass : SimpleClassBase
        {
            public int Property2 { get; set; }
        }
    }
}