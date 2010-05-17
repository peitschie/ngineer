using System;
using NGineer.BuildHelpers;
using NUnit.Framework;

namespace NGineer.UnitTests.BuildHelpers
{
    [TestFixture]
    public class PropertyMemberSetterTests
    {
        [Test]
        public void Equality_Inherited_PropertyComparison()
        {
            var parentProperty = typeof(ClassWithNullableDateTime).GetProperty("Property1");
            var inheritedProperty = typeof(InheritsFromClassWithNullableDateTime).GetProperty("Property1");
            var inheritedMember = new PropertyMemberSetter(inheritedProperty, null);
            Assert.IsTrue(inheritedMember.IsForMember(parentProperty));
        }

        [Test]
        public void Null_IsForMember()
        {
            var property = typeof(ClassWithNullableDateTime).GetProperty("Property1");
            var memberSetter = new PropertyMemberSetter(property, null);
            Assert.DoesNotThrow(() => memberSetter.IsForMember(null));
        }

        [Test]
        public void IsForMember_InheritedProperty()
        {
            var child1Property = typeof(ClassChild1).GetProperty("Property1");
            var child1PropertyMember = new PropertyMemberSetter(child1Property, null);

            var child2Property = typeof(ClassChild2).GetProperty("Property1");
            Assert.IsFalse(child1PropertyMember.IsForMember(child2Property));
        }
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

    public class ClassBuildChildren
    {
        public ClassChild1 Child1 { get; set; }
        public ClassChild2 Child2 { get; set; }
    }
}