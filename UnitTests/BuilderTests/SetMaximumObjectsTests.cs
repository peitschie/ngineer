using NGineer.Internal;
using NGineer.Utils;
using NUnit.Framework;
using NGineer.Exceptions;
using NGineer.UnitTests.Types;

namespace NGineer.UnitTests.BuilderTests
{
    [TestFixture]
    public class SetMaximumObjectsTests
    {
        [Test]
        public void DefaultValue_ThrowsExceptionWhenReached()
        {
            var builder = new Builder(1)
                .SetCollectionSize<TestClass2>(Builder.Defaults.MaximumObjects + 1, Builder.Defaults.MaximumObjects + 1)
                .Sealed();
            var exception = Assert.Throws<MaximumInstancesReachedException>(() => builder.Build<TestClass2[]>());
            Assert.AreEqual(string.Format("Maximum number of new objects was exceeded at {0} objects: TestClass2(5001)", Builder.Defaults.MaximumObjects), exception.Message);
            Assert.AreEqual(3, exception.Statistics.Count);
            int index = 0;
            Assert.AreEqual(new BuilderStatEntry(typeof(TestClass2), Builder.Defaults.MaximumObjects+1), exception.Statistics[index++]);
            Assert.AreEqual(new BuilderStatEntry(typeof(string), Builder.Defaults.MaximumObjects), exception.Statistics[index++]);
            Assert.AreEqual(new BuilderStatEntry(typeof(TestClass2[]), 1), exception.Statistics[index++]);
        }

        [Test]
        public void ManuallySet_ThrowsExceptionWhenReached()
        {
            var builder = new Builder(1)
                .SetCollectionSize<TestClass2>(10, 10)
                .SetMaximumObjects(8)
                .Sealed();
            var exception = Assert.Throws<MaximumInstancesReachedException>(() => builder.Build<TestClass2[]>());
            Assert.AreEqual("Maximum number of new objects was exceeded at 8 objects: TestClass2(9)", exception.Message);
            Assert.AreEqual(3, exception.Statistics.Count);
            int index = 0;
            Assert.AreEqual(new BuilderStatEntry(typeof(TestClass2), 9), exception.Statistics[index++]);
            Assert.AreEqual(new BuilderStatEntry(typeof(string), 8), exception.Statistics[index++]);
            Assert.AreEqual(new BuilderStatEntry(typeof(TestClass2[]), 1), exception.Statistics[index++]);
        }

        [Test]
        public void ManuallySet_NotPersistedBetweenSessions()
        {
            var builder = new Builder(1)
                .SetMaximumObjects(10)
                .SetCollectionSize<TestClass2>(8, 8)
                .Sealed();
            Assert.DoesNotThrow(() => builder.Build<TestClass2[]>());
        }

        [Test]
        public void ManuallySet_RecursiveClass_NoExceptionThrown_LessThanSpecifiedNumber()
        {
            var builder = new Builder(1)
                .SetMaximumDepth(10)
                .SetMaximumObjects(8)
                .SetNumberOfInstances<RecursiveClass>(7, 7)
                .Sealed();
            Assert.DoesNotThrow(() => builder.Build<RecursiveClass>());
        }

        [Test]
        public void ManuallySet_RecursiveClass_ExceptionThrown_MoreThanSpecifiedNumber()
        {
            var builder = new Builder(1)
                .SetMaximumObjects(3)
                .SetNumberOfInstances<RecursiveClass>(4, 4)
                .Sealed();
            var exception = Assert.Throws<MaximumInstancesReachedException>(() => builder.Build<RecursiveClass>());
            Assert.AreEqual("Maximum number of new objects was exceeded at 3 objects: RecursiveClass(4)", exception.Message);
            Assert.AreEqual(2, exception.Statistics.Count);
            int index = 0;
            Assert.AreEqual(new BuilderStatEntry(typeof(RecursiveClass), 4), exception.Statistics[index++]);
            Assert.AreEqual(new BuilderStatEntry(typeof(int), 3), exception.Statistics[index++]);
        }

        [Test]
        public void ManuallySet_ThrowsExceptionWhenReached_MultipleObjects()
        {
        	var builder = new Builder(1)
                .SetDefaultCollectionSize(10, 10)
                .SetMaximumObjects(4)
                .Sealed();
        	var exception = Assert.Throws<MaximumInstancesReachedException>(() => builder.Build<SimpleClass[]>());
        	Assert.AreEqual("Maximum number of new objects was exceeded at 4 objects: TestClass2(3)SimpleClass(2)", 
				exception.Message.Replace("\n","").Replace("\r",""));
            Assert.AreEqual(5, exception.Statistics.Count);
            int index = 0;
            Assert.AreEqual(new BuilderStatEntry(typeof(string), 5), exception.Statistics[index++]);
            Assert.AreEqual(new BuilderStatEntry(typeof(int), 3), exception.Statistics[index++]);
            Assert.AreEqual(new BuilderStatEntry(typeof(TestClass2), 3), exception.Statistics[index++]);
            Assert.AreEqual(new BuilderStatEntry(typeof(SimpleClass), 2), exception.Statistics[index++]);
            Assert.AreEqual(new BuilderStatEntry(typeof(SimpleClass[]), 1), exception.Statistics[index++]);
            
        }
    }
}