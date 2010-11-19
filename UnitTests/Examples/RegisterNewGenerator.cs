using System;
using NUnit.Framework;
using NGineer.Internal;

namespace NGineer.UnitTests.Examples
{

    public class CustomClass
    {
        public CustomClass (int specialNumber, string name)
        {
            SpecialNumber = specialNumber;
            Name = name;
        }

        public int SpecialNumber { get; private set; }
        public string Name { get; private set; }
    }

    public class CustomClassGenerator : IGenerator
    {
        // Can specifically control which types this generator will attempt to create.
        // This can re-use any information from the builder or the session, allowing
        // advanced conditions to be set-up
        public bool GeneratesType (Type type, IBuilder builder, BuildSession session)
        {
            return typeof(CustomClass).Equals (type);
        }

        // Perform any custom creation actions required before returning a
        // newly constructed object
        public ObjectBuildRecord CreateRecord(Type type, IBuilder builder, BuildSession session)
        {
            return new ObjectBuildRecord(type, new CustomClass (session.Random.Next (), "Demo Generator"), false);
        }
    }

    [TestFixture]
    public class TestCustomGenerator
    {
        [Test]
        public void Do ()
        {
            // Register a new generator to handle creating the class with no default constructor
            var customClass = new Builder (1).WithGenerator (new CustomClassGenerator ()).Build<CustomClass> ();
            Assert.AreEqual("Demo Generator", customClass.Name);
        }
    }
}
