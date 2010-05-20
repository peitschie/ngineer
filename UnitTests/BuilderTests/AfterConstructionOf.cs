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
	public class AfterConstructionOf
	{
        [Test]
        public void AfterConstructionOf_PropertiesOnlySetOnce()
        {
            var newClass = new Builder(1)
                .AfterConstructionOf<CountsPropertySets, CountsPropertySets>(c => c.RecursiveProperty, (o, b, s) => null)
                .Build<CountsPropertySets>();

            Assert.AreEqual(1, newClass.GetSomePropertySets());
            Assert.AreEqual(1, newClass.GetRecursivePropertySets());

            Assert.IsNotNull(newClass.SomeProperty);
            Assert.IsNull(newClass.RecursiveProperty);
        }

        [Test]
        public void AfterConstructionOf_NullableIntProperty()
        {
            IBuilder builder = null;
            Assert.DoesNotThrow(() => builder = new Builder(1)
                .AfterConstructionOf<ClassWithNullableInt, int?>(c => c.Property1, 9));

            var obj = builder.Build<ClassWithNullableInt>();
            Assert.AreEqual(9, obj.Property1.Value);
        }

        [Test]
        public void AfterConstructionOf_NullableDateTimeProperty()
        {
            var dateTime = DateTime.Now;
            IBuilder builder = null;
            Assert.DoesNotThrow(() => builder = new Builder(1)
                .AfterConstructionOf<ClassWithNullableDateTime, DateTime?>(c => c.Property1, dateTime));

            var obj = builder.Build<ClassWithNullableDateTime>();
            Assert.AreEqual(dateTime, obj.Property1.Value);
        }

        [Test]
        public void AfterConstructionOf_Inherited_NullableDateTimeProperty()
        {
            var dateTime = DateTime.Now;
            IBuilder builder = null;
            Assert.DoesNotThrow(() => builder = new Builder(1)
                .AfterConstructionOf<InheritsFromClassWithNullableDateTime, DateTime?>(c => c.Property1, dateTime));

            var obj = builder.Build<InheritsFromClassWithNullableDateTime>();
            Assert.AreEqual(dateTime, obj.Property1.Value);
        }
		
		[Test]
        public void AfterConstructionOf_PropertySetInSession()
        {
			List<MemberInfo> propertyOrder = new List<MemberInfo>();
            new Builder(1)
                .AfterConstructionOf<ClassWithNullableInt, int?>(c => c.Property1, 
			         (o, b, s) => {
							propertyOrder.Add(s.CurrentMember);
							return 10;
						})
				.Build<ClassWithNullableInt>();
            Assert.AreEqual(1, propertyOrder.Count);
			Assert.AreEqual(MemberTypes.Property, propertyOrder[0].MemberType);
			Assert.AreEqual("Property1", propertyOrder[0].Name);
        }

        [Test]
        public void AfterConstructionOf_MultipeSettersForProperty()
        {
            var obj = new Builder(1)
                .AfterConstructionOf<ClassWithNullableInt, int?>(c => c.Property1, (o, b, s) => 1)
                .AfterConstructionOf<ClassWithNullableInt, int?>(c => c.Property1, (o, b, s) => 2)
                .Build<ClassWithNullableInt>();
            Assert.AreEqual(2, obj.Property1);
        }

        [Test]
        public void AfterConstructionOf_Inherited_NullableDateTimeProperty_NotUsedForSibling()
        {
            var dateTime = DateTime.Now;
            IBuilder builder = null;
            Assert.DoesNotThrow(() => builder = new Builder(1)
                .AfterConstructionOf<InheritsFromClassWithNullableDateTime, DateTime?>(c => c.Property1, dateTime));

            var obj = builder.Build<InheritsFromClassWithNullableDateTime2>();
            Assert.AreNotEqual(dateTime, obj.Property1.Value);
        }

        [Test]
        public void AfterConstructionOf_InvalidMethodReturnType()
        {
            var builder = new Builder(1);
            Assert.Throws<InvalidCastException>(() => builder.AfterConstructionOf(MemberExpressions.GetMemberInfo<SimpleClass>(c => c.StringProperty), (o, b, s) => 1));
        }

        [Test]
        public void Hierarchy_AfterConstructionOf_ChildValueIsFinal()
        {
            var childCalled = -1;
            var callOrder = 0;
            var builder = new Builder(1)
                .AfterConstructionOf<SimpleClass, int>(c => c.IntProperty, (o, b, s) => 10);
            var newClass = builder.CreateNew()
                .AfterConstructionOf<SimpleClass, int>(c => c.IntProperty,
                (o, b, s) =>
                {
                    childCalled = callOrder++;
                    return 11;
                })
                .Build<SimpleClass>();

            Assert.AreNotEqual(-1, childCalled, "Child wasn't called");
            Assert.AreEqual(11, newClass.IntProperty);
        }
		
        [Test]
        public void Hierarchy_AfterConstructionOf_BothParentsAndChildrenCalled()
        {
            var obj = new Builder(1)
                .AfterConstructionOf<SimpleClass, int>(c => c.IntProperty, (o, b, s) => 10)
				.CreateNew()
                .AfterConstructionOf<SimpleClass, string>(c => c.StringProperty, (o, b, s) => "childValue")
                .Build<SimpleClass>();

            Assert.AreEqual(10, obj.IntProperty);
			Assert.AreEqual("childValue", obj.StringProperty);
        }
		
        [Test]
        public void Hierarchy_AfterConstructionOf_BuildingInParentSession()
        {
            var obj = new Builder(1)
                .AfterConstructionOf<SimpleClass, TestClass2>(c => c.TestClass2Property, (o, b, s) => b.Build<TestClass2>(s))
				.CreateNew()
                .AfterConstructionOf<TestClass2, string>(c => c.StringProperty, (o, b, s) => "childValue")
                .Build<SimpleClass>();

			Assert.AreEqual("childValue", obj.TestClass2Property.StringProperty);
        }
	}
}
