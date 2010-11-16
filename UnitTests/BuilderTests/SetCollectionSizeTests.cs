
using System;
using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using NGineer.UnitTests.Types;

namespace NGineer.UnitTests.BuilderTests
{
	[TestFixture]
	public class SetCollectionSizeTests
	{
		[Test]
        public void SetCollectionSize_ObjectCollectionProperlySet_Array()
        {
            var builder = new Builder(1)
                .SetCollectionSize<SimpleClass>(3, 3)
                .Sealed();
            var simpleClassArray = builder.Build<SimpleClass[]>();
            Assert.AreEqual(3, simpleClassArray.Length);
        }

        [Test]
        public void SetCollectionSize_ObjectCollectionProperlySet_List()
        {
            var builder = new Builder(1)
                .SetCollectionSize<SimpleClass>(3, 3)
                .Sealed();
            var simpleClassArray = builder.Build<List<SimpleClass>>();
            Assert.AreEqual(3, simpleClassArray.Count);
        }

        [Test]
        public void SetCollectionSize_ObjectCollectionProperlySet_LastSettingOverridesPrevious()
        {
            var builder = new Builder(1)
                .SetCollectionSize<SimpleClass>(10, 10)
                .SetCollectionSize<SimpleClass>(3, 3)
                .Sealed();
            var simpleClassArray = builder.Build<SimpleClass[]>();
            Assert.AreEqual(3, simpleClassArray.Length);
        }


        [Test]
        public void SetCollectionSize_ObjectCollectionProperlySet_InChildBuilder()
        {
            var builder = new Builder(1)
                .SetCollectionSize<SimpleClass>(3, 3)
                .For<SimpleClassParent>()
                    .Set(x => x.Child, (obj, buildr, session) => buildr.CreateNew().Build<SimpleClass[]>())
                    .Ignore(x => x.UniqueChild)
                .Sealed();
            var parentClass = builder.Build<SimpleClassParent>();
            Assert.AreEqual(3, parentClass.Child.Length);
        }

        [Test]
        public void SetCollectionSize_ObjectsProperlyReused_ChildBuilder_WithinChildBuilder()
        {
            var builder = new Builder(1)
                .SetCollectionSize<SimpleClass>(3, 3)
                .CreateNew()
                .For<SimpleClassParent>()
                    .Set(x => x.Child, (obj, buildr, session) => buildr.CreateNew().Build<SimpleClass[]>())
                    .Ignore(x => x.UniqueChild)
                .Sealed();
            var parentClass = builder.Build<SimpleClassParent>();
            Assert.AreEqual(3, parentClass.Child.Length);
        }

        [Test]
        public void SetCollectionSize_ObjectsProperlyReused_ChildBuilder_WithinChildBuilder_OverridesParent()
        {
            var builder = new Builder(1)
                .SetCollectionSize<SimpleClass>(10, 10)
                .CreateNew()
                .SetCollectionSize<SimpleClass>(3, 3)
                .For<SimpleClassParent>()
                    .Set(x => x.Child, (obj, buildr, session) => buildr.CreateNew().Build<SimpleClass[]>())
                    .Ignore(x => x.UniqueChild)
                .Sealed();
            var parentClass = builder.Build<SimpleClassParent>();
            Assert.AreEqual(3, parentClass.Child.Length);
        }
	}
}
