using NGineer.BuildHelpers;
using NUnit.Framework;

namespace NGineer.UnitTests.BuildHelpers
{
    [TestFixture]
    public class ObjectBuildTreeEntryTests
    {
        private ObjectBuildTreeEntry _treeEntry;

        [Test]
        public void NullRecord_PropertyAccess()
        {
            _treeEntry = new ObjectBuildTreeEntry(null, null, 1);
            object storage;
            Assert.DoesNotThrow(() => storage = _treeEntry.Type);
            Assert.DoesNotThrow(() => storage = _treeEntry.Record);
            Assert.DoesNotThrow(() => storage = _treeEntry.Parent);
            Assert.DoesNotThrow(() => storage = _treeEntry.IsPopulated);
            Assert.DoesNotThrow(() => storage = _treeEntry.UnconstructedMembers);
            Assert.DoesNotThrow(() => storage = _treeEntry.Depth);
            Assert.DoesNotThrow(() => storage = _treeEntry.Children);
        }
        
    }
}