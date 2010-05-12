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
}