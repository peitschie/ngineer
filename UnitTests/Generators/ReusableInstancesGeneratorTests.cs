using System.Collections.Generic;
using System;
using System.Linq;
using Moq;
using NGineer.BuildGenerators;
using NGineer.BuildHelpers;
using NGineer.SpecialGenerators;
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

        private static void AddConstructedNode<TType>(TType obj, BuildSession session)
        {
            session.ConstructedNodes.Add(new ObjectBuildTreeEntry(null, new ObjectBuildRecord(typeof(TType), obj), 0));
        }

        [Test]
        public void GeneratesType_AfterMaxNumberOfSessionsReached()
        {
            var session = CreateSession();
            Generator.SetNumberOfInstances<int>(3, 3);

            AddConstructedNode(1, session);
            Assert.IsFalse(Generator.GeneratesType(typeof(int), null, session));

            AddConstructedNode(2, session);
            Assert.IsFalse(Generator.GeneratesType(typeof(int), null, session));

            AddConstructedNode(3, session);
            Assert.IsTrue(Generator.GeneratesType(typeof(int), null, session));
        }

        [Test]
        public void Create_ReturnsExisting_AfterMaxNumberOfSessionsReached()
        {
            var session = CreateSession();
            Generator.SetNumberOfInstances<SimpleClass>(3, 3);

            AddConstructedNode(new SimpleClass(), session);
            AddConstructedNode(new SimpleClass(), session);
            AddConstructedNode(new SimpleClass(), session);

            for (int i = 0; i < 20; i++ )
            {
                var obj = CreateAndGenerate<SimpleClass>(null, session);
                var foundObj = session.ConstructedNodes.FirstOrDefault(constructedObject => ReferenceEquals(obj.Object, constructedObject.Object));
                Assert.IsNotNull(foundObj, "Unknown instance returned");
            }
        }

        [Test]
        public void Create_NullsInBuildSession_NoExceptionsThrown()
        {
            var session = CreateSession();
            Generator.SetNumberOfInstances<SimpleClass>(3, 3);

            AddConstructedNode(new SimpleClass(), session);
            AddConstructedNode<SimpleClass>(null, session);
            AddConstructedNode(new SimpleClass(), session);
            AddConstructedNode(new SimpleClass(), session);

            Assert.DoesNotThrow(() => CreateAndGenerate<SimpleClass>(null, session));
        }

        private new ObjectBuildRecord CreateAndGenerate<TType>(IBuilder builder, BuildSession session)
        {
            var obj = Generator.Create(typeof(TType), builder, session);
            Generator.Populate(typeof(TType), obj, builder, session);
            return obj as ObjectBuildRecord;
        }

        private static BuildSession CreateSession()
        {
            return new BuildSession(null, null, null);
        }

        public class SimpleClass {}
        
    }
}