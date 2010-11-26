using System;
using System.Xml.Serialization;
using System.Reflection;
using NUnit.Framework;
namespace NGineer.UnitTests.Examples
{
    public class XmlClass
    {
        [XmlIgnore]
        public string XmlIgnoredProperty { get; set; }

        public string ConstructedProperty { get; set; }
    }

    public class IgnoreXmlIgnoredAttributeSetter : IMemberSetter
    {
        public bool IsForMember(MemberInfo member, IBuilder builder, BuildSession session)
        {
            // Return true for any member that has an XmlIgnore attribute on it.
            // This signifies to NGineer that this setter will consume the given member and prevent
            // any other member setter from being run on it
            return member.GetCustomAttributes(typeof(XmlIgnoreAttribute), true).Length > 0;
        }

        public void Set(object obj, IBuilder builder, BuildSession session)
        {
            // Perform no population.  Simple consume these members and leave them untouched
        }
    }

    [TestFixture]
    public class XmlIgnoredAttributeExample
    {
        [Test]
        public void Do ()
        {
            // Register a new member setter to handle xml ignored properties
            var customClass = new Builder()
                // AfterConstructionOf setters are run after an object is constructed by a generator
                // but before any instance processors are called
                .AfterConstructionOf(new IgnoreXmlIgnoredAttributeSetter())
                .Build<XmlClass>();
            Assert.IsNotNull(customClass.ConstructedProperty);
            Assert.IsNull(customClass.XmlIgnoredProperty);
        }
    }
}

