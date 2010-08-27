using System;
using System.Collections;
using System.Linq;
using NGineer.BuildHelpers;
using NGineer.Utils;
using NUnit.Framework;
using System.Collections.Generic;
using System.Reflection;
using NGineer.BuildGenerators;


namespace NGineer.UnitTests.BuilderTests
{
	[TestFixture]
	public class AfterConstructionOfTests
	{
		[Test]
        public void AfterConstructionOf_PropertySetter_NullMemberSetter()
        {
            Assert.Throws<ArgumentNullException>(() => new Builder(1)
                .AfterConstructionOf((IMemberSetter)null));
        }
		
		[Test]
        public void AfterConstructionOf_PropertySetter_NullSetter()
        {
            Assert.Throws<ArgumentNullException>(() => new Builder(1)
                .For<SimpleClass>().Set(c => c.StringProperty, 
			    	(Func<SimpleClass, IBuilder, BuildSession, string>)null));
        }
		
		[Test]
        public void AfterConstructionOf_PropertySetter_NullMemberInfo()
        {
            Assert.Throws<ArgumentNullException>(() => new Builder(1)
                .For<SimpleClass>().Set(null, (o, b, s) => "string"));
        }
		
		[Test]
        public void AfterConstructionOf_PropertySetter_NullLambdaSetterValue()
        {
        	var builder = new Builder(1)
                .For<SimpleClass>()
					.Set(c => c.StringProperty, (o, b, s) => null)
					.Set(c => c.StringField, (o, b, s) => null)
				;
            SimpleClass newClass = null;
			Assert.DoesNotThrow(() => newClass = builder.Build<SimpleClass>());

            Assert.IsNotNull(newClass);
			Assert.IsNull(newClass.StringProperty);
			Assert.IsNull(newClass.StringField);
        }
		
		[Test]
        public void AfterConstructionOf_PropertySetter_NullValue()
        {
        	var builder = new Builder(1)
                .For<SimpleClass>()
					.Set(c => c.StringProperty, (string)null)
					.Set(c => c.StringField, (string)null)
				;
            SimpleClass newClass = null;
			Assert.DoesNotThrow(() => newClass = builder.Build<SimpleClass>());

            Assert.IsNotNull(newClass);
			Assert.IsNull(newClass.StringProperty);
			Assert.IsNull(newClass.StringField);
        }
		
        [Test]
        public void AfterConstructionOf_PropertiesOnlySetOnce()
        {
            var newClass = new Builder(1)
                .For<CountsPropertySets>().Set(c => c.RecursiveProperty, (o, b, s) => null)
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
                .For<ClassWithNullableInt>().Set(c => c.Property1, 9));

            var obj = builder.Build<ClassWithNullableInt>();
            Assert.AreEqual(9, obj.Property1.Value);
        }

        [Test]
        public void AfterConstructionOf_NullableDateTimeProperty()
        {
            var dateTime = DateTime.Now;
            IBuilder builder = null;
            Assert.DoesNotThrow(() => builder = new Builder(1)
                .For<ClassWithNullableDateTime>().Set(c => c.Property1, dateTime));

            var obj = builder.Build<ClassWithNullableDateTime>();
            Assert.AreEqual(dateTime, obj.Property1.Value);
        }

        [Test]
        public void AfterConstructionOf_Inherited_NullableDateTimeProperty()
        {
            var dateTime = DateTime.Now;
            IBuilder builder = null;
            Assert.DoesNotThrow(() => builder = new Builder(1)
                .For<InheritsFromClassWithNullableDateTime>().Set(c => c.Property1, dateTime));

            var obj = builder.Build<InheritsFromClassWithNullableDateTime>();
            Assert.AreEqual(dateTime, obj.Property1.Value);
        }
		
		[Test]
        public void AfterConstructionOf_PropertySetInSession()
        {
			List<MemberInfo> propertyOrder = new List<MemberInfo>();
            new Builder(1)
                .For<ClassWithNullableInt>().Set(c => c.Property1, 
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
                .For<ClassWithNullableInt>()
					.Set(c => c.Property1, (o, b, s) => 1)
                	.Set(c => c.Property1, (o, b, s) => 2)
                .Build<ClassWithNullableInt>();
            Assert.AreEqual(2, obj.Property1);
        }

        [Test]
        public void AfterConstructionOf_Inherited_NullableDateTimeProperty_NotUsedForSibling()
        {
            var dateTime = DateTime.Now;
            IBuilder builder = null;
            Assert.DoesNotThrow(() => builder = new Builder(1)
                .For<InheritsFromClassWithNullableDateTime>().Set(c => c.Property1, dateTime));

            var obj = builder.Build<InheritsFromClassWithNullableDateTime2>();
            Assert.AreNotEqual(dateTime, obj.Property1.Value);
        }

        [Test]
        public void Hierarchy_AfterConstructionOf_ChildValueIsFinal()
        {
        	var childPropertySetterCalled = -1;
        	var childFieldSetterCalled = -1;
        	var callOrder = 0;
        	var builder = new Builder(1)
                .For<SimpleClass>()
						.Set(c => c.IntProperty, (o, b, s) => 10)
                		.Set(c => c.IntField, (o, b, s) => 10)
                .CreateNew()
                .For<SimpleClass>().Set(c => c.IntProperty,
                (o, b, s) =>
                {
                    childPropertySetterCalled = callOrder++;
                    return 11;
                })
                .Set(c => c.IntField,
                (o, b, s) =>
                {
                    childFieldSetterCalled = callOrder++;
                    return 11;
                })
                .Sealed();
            var newClass = builder.Build<SimpleClass>();

            Assert.AreNotEqual(-1, childPropertySetterCalled, "Child property setter wasn't called");
            Assert.AreNotEqual(-1, childFieldSetterCalled, "Child field setter wasn't called");
            Assert.AreEqual(11, newClass.IntProperty);
            Assert.AreEqual(11, newClass.IntField);
        }
		
        [Test]
        public void Hierarchy_AfterConstructionOf_BothParentsAndChildrenCalled()
        {
        	var obj = new Builder(1)
                .For<SimpleClass>()
					.Set(c => c.IntProperty, (o, b, s) => 10)
					.Set(c => c.IntField, (o, b, s) => 11)
				.CreateNew()
                .For<SimpleClass>()
					.Set(c => c.StringProperty, (o, b, s) => "childValue")
					.Set(c => c.StringField, (o, b, s) => "childValueField")
                .Build<SimpleClass>();

            Assert.AreEqual(10, obj.IntProperty);
			Assert.AreEqual("childValue", obj.StringProperty);
			Assert.AreEqual(11, obj.IntField);
			Assert.AreEqual("childValueField", obj.StringField);
        }
		
        [Test]
        public void Hierarchy_AfterConstructionOf_BuildingInParentSession()
        {
            var obj = new Builder(1)
                .For<SimpleClass>().Set(c => c.TestClass2Property, (o, b, s) => b.Build<TestClass2>(s))
				.CreateNew()
                .For<TestClass2>().Set(c => c.StringProperty, (o, b, s) => "childValue")
                .Build<SimpleClass>();

			Assert.AreEqual("childValue", obj.TestClass2Property.StringProperty);
        }
		
		[Test]
        public void Hierarchy_AfterConstructionOf_BuildingInParentSession_CreateNew()
        {
            var obj = new Builder(1)
                .For<SimpleClass>().Set(c => c.TestClass2Property, (o, b, s) => {
					return b
						.CreateNew()
						.For<TestClass2>().Set(c => c.StringProperty, (o1, b1, s1) => "new value")
						.Build<TestClass2>(s); })
				.CreateNew()
                .For<TestClass2>().Set(c => c.StringProperty, (o, b, s) => "childValue")
                .Build<SimpleClass>();

			Assert.AreEqual("new value", obj.TestClass2Property.StringProperty);
        }
		
		[Test]
        public void Hierarchy_AfterConstructionOf_BuildingInParentSession_CreateNew_CanOverride()
        {
        	var obj = new Builder(1)
                .For<SimpleClass>().Set(c => c.TestClass2Property, (o, b, s) => {
					return b
						.CreateNew()
						.For<TestClass2>()
							.Set(c => c.StringProperty, (o1, b1, s1) => "new value")
						.Build<TestClass2>(s); })
				.CreateNew()
				.For<SimpleClass>().Set(c => c.TestClass2Property, (o, b, s) => b.Build<TestClass2>())
                .For<TestClass2>().Set(c => c.StringProperty, (o, b, s) => "childValue")
                .Build<SimpleClass>();

			Assert.AreEqual("childValue", obj.TestClass2Property.StringProperty);
        }

        [Test]
        public void UniqueCollection_SyntaxExample_UsingAfterConstructionOf()
        {
            var builder = new Builder(1)
                .SetDefaultCollectionSize(10,10)
                .AfterConstructionOf<ListOfClassWithEnumAndProperties>(
                    c => c.Entries,
                    Generators.UniqueCollection.ForMember<ClassWithEnumAndProperties>(c => c.EnumProperty));

            var list = builder.Build<ListOfClassWithEnumAndProperties>();

            Assert.AreEqual(EnumUtils.GetValues<SimpleEnum>().Count(), list.Entries.Count);
        }

        [Test]
        public void AfterConstructionOf_IgnoreProperty()
        {
            var builder = new Builder(1)
                .For<SimpleClass>().Ignore(c => c.StringProperty)
                ;
            SimpleClass newClass = null;
            Assert.DoesNotThrow(() => newClass = builder.Build<SimpleClass>());

            Assert.IsNotNull(newClass);
            Assert.IsNull(newClass.StringProperty);
            Assert.IsNotNull(newClass.StringField);
        }
	}
}
