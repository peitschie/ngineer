using System;
using System.Reflection;
using NGineer.BuildGenerators;
using NGineer.BuildHelpers;
using NGineer.Utils;
using NUnit.Framework;

namespace NGineer.UnitTests.BuilderTests
{
    [TestFixture]
    public class SealedBuilderTests
    {
        private IBuilder _builder;

        [SetUp]
        public void SetUp()
        {
            _builder = new Builder().Sealed();
        }

        [Test]
        public void AfterConstructionOf()
        {
            Assert.Throws<BuilderSealedException>(() => _builder.AfterConstructionOf(new TestMemberSetter()));
        }

        [Test]
        public void WithGenerator()
        {
            Assert.Throws<BuilderSealedException>(() => _builder.WithGenerator(new TestGenerator()));
        }

        [Test]
        public void SetMaximumDepth()
        {
            Assert.Throws<BuilderSealedException>(() => _builder.SetMaximumDepth(null));
        }

        [Test]
        public void SetBuildOrder()
        {
            Assert.Throws<BuilderSealedException>(() => _builder.SetBuildOrder(null));
        }

        [Test]
        public void SetMaximumObjects()
        {
            Assert.Throws<BuilderSealedException>(() => _builder.SetMaximumObjects(null));
        }

        [Test]
        public void ThrowsWhenMaximumDepthReached()
        {
            Assert.Throws<BuilderSealedException>(() => _builder.ThrowsWhenMaximumDepthReached());
        }

        [Test]
        public void SetDefaultCollectionSize()
        {
            Assert.Throws<BuilderSealedException>(() => _builder.SetDefaultCollectionSize(0,0));
        }

        [Test]
        public void SetCollectionSize()
        {
            Assert.Throws<BuilderSealedException>(() => _builder.SetCollectionSize(typeof(string), 0, 0));
        }

        [Test]
        public void SetNumberOfInstances()
        {
            Assert.Throws<BuilderSealedException>(() => _builder.SetNumberOfInstances(typeof(string), 0, 0));
        }

        [Test]
        public void AfterPopulationOf()
        {
            Assert.Throws<BuilderSealedException>(() => _builder.AfterPopulationOf(new TestSetter()));
        }

        [Test]
        public void IgnoreMember()
        {
            Assert.Throws<BuilderSealedException>(() => _builder.IgnoreMember(MemberExpressions.GetMemberInfo<string>(c => c.Length), true));
        }

        [Test]
        public void IgnoreUnset()
        {
            Assert.Throws<BuilderSealedException>(() => _builder.IgnoreUnset(typeof(string)));
        }

        [Test]
        public void For()
        {
            Assert.Throws<BuilderSealedException>(() => _builder.For<string>());
        }

        [Test]
        public void PostBuild()
        {
            Assert.Throws<BuilderSealedException>(() => _builder.PostBuild(o => {}));
        }

        [Test]
        public void Sealed()
        {
            Assert.DoesNotThrow(() => _builder.Sealed());
        }

        [Test]
        public void CreateNew()
        {
            Assert.DoesNotThrow(() => _builder.CreateNew());
        }

        [Test]
        public void Build()
        {
            Assert.DoesNotThrow(() => _builder.Build(typeof(string)));
        }

        private class TestMemberSetter : IMemberSetter
        {
            public bool IsForMember(MemberInfo member, IBuilder builder, BuildSession session)
            {
                throw new NotImplementedException();
            }

            public void Set(object obj, IBuilder builder, BuildSession session)
            {
                throw new NotImplementedException();
            }
        }

        public class TestSetter : ISetter
        {
            public bool IsForType(Type type)
            {
                throw new NotImplementedException();
            }

            public object Set(object obj, IBuilder builder, BuildSession session)
            {
                throw new NotImplementedException();
            }
        }

        private class TestGenerator : IGenerator
        {
            public bool GeneratesType(Type type, IBuilder builder, BuildSession session)
            {
                throw new NotImplementedException();
            }

            public object Create(Type type, IBuilder builder, BuildSession session)
            {
                throw new NotImplementedException();
            }

            public void Populate(Type type, object obj, IBuilder builder, BuildSession session)
            {
                throw new NotImplementedException();
            }
        }
    }
}