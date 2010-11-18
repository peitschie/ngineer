using NGineer.Internal;
using NUnit.Framework;

namespace NGineer.UnitTests.Internal
{
    [TestFixture]
    public class ObjectBuildTreeEntryTests
    {
        private ObjectBuildTreeEntry _treeEntry;

        [Test]
        public void NullRecord_PropertyAccess()
        {
            _treeEntry = new ObjectBuildTreeEntry(null, null, 1);
            object storage = null;
            Assert.DoesNotThrow(() => storage = _treeEntry.Type);
            Assert.DoesNotThrow(() => storage = _treeEntry.Record);
            Assert.DoesNotThrow(() => storage = _treeEntry.Parent);
            Assert.DoesNotThrow(() => storage = _treeEntry.UnconstructedMembers);
            Assert.DoesNotThrow(() => storage = _treeEntry.Depth);
            Assert.DoesNotThrow(() => storage = _treeEntry.Children);
            Assert.IsNotNull(storage); // Always true... this stops the warning about the variable never being used
        }
        
    }
}