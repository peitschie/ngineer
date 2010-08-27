using System;
using System.Collections;
using System.Linq;
using NGineer.BuildHelpers;
using NGineer.Utils;
using NUnit.Framework;
using System.Collections.Generic;
using System.Reflection;


namespace NGineer.UnitTests.BuilderTests
{
	[TestFixture]
	public class AfterPopulationOfTests
	{
		[Test]
        public void AfterPopulationOf_Action_NoBuilder()
        {
            Assert.Throws<ArgumentNullException>(() => new Builder(1)
                .AfterPopulationOf<SimpleClass>((Action<SimpleClass>)null));
        }
		
		[Test]
        public void AfterPopulationOf_Action_WithBuilder()
        {
            Assert.Throws<ArgumentNullException>(() => new Builder(1)
                .AfterPopulationOf<SimpleClass>((Action<SimpleClass,IBuilder,BuildSession>)null));
        }
		
		[Test]
        public void AfterPopulationOf_ISetter()
        {
            Assert.Throws<ArgumentNullException>(() => new Builder(1)
                .AfterPopulationOf((ISetter)null));
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
        public void Build_SettersAreProperlyCalled_SimpleInt()
        {
            var newClass = new Builder(1).AfterPopulationOf<SimpleClass>(n => n.IntProperty = 190).Build<SimpleClass>();
            Assert.AreEqual(190, newClass.IntProperty);
        }

        [Test]
        public void Build_SetupValueToOverrideBehaviour_SimpleClass()
        {
            var newClass = new Builder(1)
                .AfterPopulationOf<SimpleClass>((t, b, s) => t.StringProperty = b.Build<string>())
                .Build<SimpleClass>();

            Assert.IsNotNull(newClass.StringProperty);
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
    }
}
