using System;
using NGineer.BuildHelpers;
using NUnit.Framework;

namespace NGineer.UnitTests.BuildHelpers
{
    [TestFixture]
    public class MemberSetterTests
    {
        [Test]
        public void InheritedProperty_ChildForParent()
        {
            var expectedDateTime = DateTime.Now;
            var member = new InheritsFromClassWithNullableDateTime();

            var inheritedProperty = typeof(InheritsFromClassWithNullableDateTime).GetProperty("Property1");
            var parentProperty = typeof(ClassWithNullableDateTime).GetProperty("Property1");
            var parentMember = new MemberSetter(parentProperty, (o, b, s) => expectedDateTime);
            Assert.IsTrue(parentMember.IsForMember(inheritedProperty, null, null));
            
            parentMember.Set(member, null, null);
            Assert.AreEqual(expectedDateTime, member.Property1);
        }

        [Test]
        public void InheritedProperty_ChildNotForParent()
        {
            var inheritedProperty = typeof(InheritsFromClassWithNullableDateTime).GetProperty("Property1");
            var parentProperty = typeof(ClassWithNullableDateTime).GetProperty("Property1");
            var inheritedMember = new MemberSetter(inheritedProperty, (o, b, s) => null);
            Assert.IsFalse(inheritedMember.IsForMember(parentProperty, null, null));
        }

        [Test]
        public void InheritedProperty_NotForSibling()
        {
            var child1Property = typeof(ClassChild1).GetProperty("Property1");
            var child1PropertyMember = new MemberSetter(child1Property, (o, b, s) => null);

            var child2Property = typeof(ClassChild2).GetProperty("Property1");
            Assert.IsFalse(child1PropertyMember.IsForMember(child2Property, null, null));
        }

        [Test]
        public void InheritedProperty_ParentIsForChildren()
        {
            const int number = 1;
            var child1 = new ClassChild1();
            var child2 = new ClassChild2();

            var parentProperty = typeof(ClassParent).GetProperty("Property1");
            var parentPropertyMember = new MemberSetter(parentProperty, (o, b, s) => number);
            var child1Property = typeof(ClassChild1).GetProperty("Property1");
            var child2Property = typeof(ClassChild2).GetProperty("Property1");

            Assert.IsTrue(parentPropertyMember.IsForMember(child1Property, null, null));
            Assert.IsTrue(parentPropertyMember.IsForMember(child2Property, null, null));

            parentPropertyMember.Set(child1, null, null);
            Assert.AreEqual(number, child1.Property1);

            parentPropertyMember.Set(child2, null, null);
            Assert.AreEqual(number, child2.Property1);
        }

        [Test]
        public void Equality_SameObject_PropertyComparison()
        {
            var classWithNullable = new ClassWithNullableDateTime();
            var expected = DateTime.Now;

            var property1Property = typeof(ClassWithNullableDateTime).GetProperty("Property1");
            var property1Member = new MemberSetter(property1Property, (o, b, s) => expected);
            var property2Property = typeof(ClassWithNullableDateTime).GetProperty("Property1");
            Assert.IsTrue(property1Member.IsForMember(property2Property, null, null));

            property1Member.Set(classWithNullable, null, null);
            Assert.AreEqual(expected, classWithNullable.Property1);
        }

        [Test]
        public void Null_IsForMember()
        {
            var property = typeof(ClassWithNullableDateTime).GetProperty("Property1");
            var memberSetter = new MemberSetter(property, (o, b, s) => null);
            Assert.DoesNotThrow(() => memberSetter.IsForMember(null, null, null));
        }

        public class ClassWithNullableInt
        {
            public int? Property1 { get; set; }
        }

        public class ClassWithNullableDateTime
        {
            public DateTime? Property1 { get; set; }
        }

        public class InheritsFromClassWithNullableDateTime : ClassWithNullableDateTime
        {
        }

        public class ClassParent
        {
            public int Property1 { get; set; }
        }

        public class ClassChild1 : ClassParent { }
        public class ClassChild2 : ClassParent { }
    }

	[TestFixture]
    public class MemberSetterTestsGenerics
    {
        [Test]
        public void InheritedProperty_NotForSibling()
        {
            var child1Property = typeof(GenericClassChild1).GetProperty("Property1");
            var child1PropertyMember = new MemberSetter(child1Property, (o, b, s) => null);

            var child2Property = typeof(GenericClassChild2).GetProperty("Property1");
            Assert.IsFalse(child1PropertyMember.IsForMember(child2Property, null, null));
        }

        [Test]
        public void InheritedProperty_ParentIsForChildren()
        {
            var parentProperty = typeof(GenericClassParent<int>).GetProperty("Property1");
            var parentPropertyMember = new MemberSetter(parentProperty, (o, b, s) => null);
            var child1Property = typeof(GenericClassChild1).GetProperty("Property1");
            var child2Property = typeof(GenericClassChild2).GetProperty("Property1");

            Assert.IsTrue(parentPropertyMember.IsForMember(child1Property, null, null));
            Assert.IsTrue(parentPropertyMember.IsForMember(child2Property, null, null));
        }

        [Test]
        public void Equality_SameObject_PropertyComparison()
        {
            var property1Property = typeof(GenericClassChild1).GetProperty("Property1");
            var property1Member = new MemberSetter(property1Property, (o, b, s) => null);
            var property2Property = typeof(GenericClassChild1).GetProperty("Property1");
            Assert.IsTrue(property1Member.IsForMember(property2Property, null, null));
        }

        public class GenericClassParent<TType>
        {
            public TType Property1 { get; set; }
        }

        public class GenericClassChild1 : GenericClassParent<int> { }
        public class GenericClassChild2 : GenericClassParent<int> { }
    }
}