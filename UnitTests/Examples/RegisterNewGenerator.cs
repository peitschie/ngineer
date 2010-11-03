using System;
using NUnit.Framework;
using NGineer.BuildGenerators;
using NGineer.BuildHelpers;

namespace NGineer.UnitTests
{

    public class CustomClass
    {
        private readonly string _name;

        public CustomClass (int specialNumber, string name)
        {
            SpecialNumber = specialNumber;
            _name = name;
        }

        public int SpecialNumber { get; private set; }
        public string Name { get { return _name; } }
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
        public object Create (Type type, IBuilder builder, BuildSession session)
        {
            return new CustomClass (session.Random.Next (), "Demo Generator");
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
