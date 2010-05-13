using System.Collections.Generic;
using System;
using System.Linq;
using Moq;
using NGineer.BuildHelpers;
using NGineer.Generators;
using NUnit.Framework;

namespace NGineer.UnitTests.Generators
{
    [TestFixture]
    public class ReusableInstancesGeneratorTests : GeneratorTestFixture<ReusableInstancesGenerator>
    {
        protected override Type[] SupportedTypes()
        {
            return new Type[0];
        }

        protected override Type[] UnsupportedTypes()
        {
            return new[]
                {
                    typeof (int),
                    typeof (string),
                    typeof (DateTime?),
                    typeof (DateTime[]),
                };
        }

        [Test]
        public void GeneratesType_AfterMaxNumberOfSessionsReached()
        {
            var session = new BuildSession(null, null);
            Generator.SetNumberOfInstances<int>(3, 3);

            session.ConstructedObjects.Add(1);
            Assert.IsFalse(Generator.GeneratesType(typeof(int), null, session));

            session.ConstructedObjects.Add(2);
            Assert.IsFalse(Generator.GeneratesType(typeof(int), null, session));

            session.ConstructedObjects.Add(3);
            Assert.IsTrue(Generator.GeneratesType(typeof(int), null, session));
        }

        [Test]
        public void Create_ReturnsExisting_AfterMaxNumberOfSessionsReached()
        {
            var session = new BuildSession(null, null);
            Generator.SetNumberOfInstances<SimpleClass>(3, 3);

            session.ConstructedObjects.Add(new SimpleClass());
            session.ConstructedObjects.Add(new SimpleClass());
            session.ConstructedObjects.Add(new SimpleClass());

            for (int i = 0; i < 20; i++ )
            {
                var obj = CreateAndGenerate<SimpleClass>(null, session);
                var foundObj = session.ConstructedObjects.FirstOrDefault(constructedObject => ReferenceEquals(obj, constructedObject));
                Assert.IsNotNull(foundObj, "Unknown instance returned");
            }
        }

        [Test]
        public void Create_NullsInBuildSession_NoExceptionsThrown()
        {
            var session = new BuildSession(null, null);
            Generator.SetNumberOfInstances<SimpleClass>(3, 3);

            session.ConstructedObjects.Add(new SimpleClass());
            session.ConstructedObjects.Add(null);
            session.ConstructedObjects.Add(new SimpleClass());
            session.ConstructedObjects.Add(new SimpleClass());

            Assert.DoesNotThrow(() => CreateAndGenerate<SimpleClass>(null, session));
        }

        public class SimpleClass {}
        
    }
}