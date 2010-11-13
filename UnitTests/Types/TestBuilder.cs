using System;
using Moq;
using NGineer.BuildGenerators;
using NGineer.Internal;
using NGineer.Utils;
using NUnit.Framework;
using Range = NGineer.Internal.Range;
using System.Collections.Generic;
namespace NGineer.UnitTests.Types
{
    public class TestBuilder : IConfiguredBuilder, IBuilder
    {
        public IEnumerable<Action<BuildSession>> PostBuildHooks { get; set; }
        public bool ThrowWhenBuildDepthReached { get; set; }
        public bool IsBuildDepthUnset { get; set; }
        public int MaximumObjects { get; set; }
        public Range DefaultCollectionSize { get; set; }
        public ITypeRegistry<Range> CollectionSizes { get; set; }
        public ITypeRegistry<int?> MaxInstances { get; set; }
        public IConfiguredBuilder Parent { get; set; }
        public IEnumerable<IInstanceProcessor> Setters { get; set; }
        public IEnumerable<IMemberSetter> MemberSetters { get; set; }

        public object Build(Type type)
        {
            throw new NotImplementedException();
        }

        public TType Build<TType>()
        {
            throw new NotImplementedException();
        }

        public TType Build<TType>(BuildSession session)
        {
            throw new NotImplementedException();
        }

        public object Build(Type type, BuildSession session)
        {
            throw new NotImplementedException();
        }

        public IBuilder SetMaximumDepth(Nullable<int> depth)
        {
            throw new NotImplementedException();
        }

        public IBuilder SetMaximumObjects(Nullable<int> maximum)
        {
            throw new NotImplementedException();
        }

        public IBuilder ThrowsWhenMaximumDepthReached()
        {
            throw new NotImplementedException();
        }

        public IBuilder WithGenerator(IGenerator generator)
        {
            throw new NotImplementedException();
        }

        public IBuilder WithGenerator(Type type, Func<IBuilder, BuildSession, object> generator)
        {
            throw new NotImplementedException();
        }

        public IBuilder WithGenerator<TType>(Func<IBuilder, BuildSession, TType> generator)
        {
            throw new NotImplementedException();
        }

        public IBuilder WithGenerator<TType>(Func<TType> generator)
        {
            throw new NotImplementedException();
        }

        public IBuilder AfterConstructionOf(IMemberSetter setter)
        {
            throw new NotImplementedException();
        }

        public IBuilder IgnoreUnset(Type type)
        {
            throw new NotImplementedException();
        }

        public IBuilder AfterPopulationOf(IInstanceProcessor setter)
        {
            throw new NotImplementedException();
        }

        public IBuilder SetDefaultCollectionSize(int minimum, int maximum)
        {
            throw new NotImplementedException();
        }

        public IBuilder SetCollectionSize(Type type, int minimum, int maximum)
        {
            throw new NotImplementedException();
        }

        public IBuilder SetCollectionSize<TType>(int minimum, int maximum)
        {
            throw new NotImplementedException();
        }

        public IBuilder SetNumberOfInstances(Type type, int minimum, int maximum)
        {
            throw new NotImplementedException();
        }

        public IBuilder SetNumberOfInstances<TType>(int minimum, int maximum)
        {
            throw new NotImplementedException();
        }

        public ITypedBuilder<TType> For<TType>()
        {
            throw new NotImplementedException();
        }

        public ITypedBuilder<TType> For<TType>(bool allowInherited)
        {
            throw new NotImplementedException();
        }

        public IBuilder PostBuild(Action<BuildSession> hook)
        {
            throw new NotImplementedException();
        }

        public IBuilder CreateNew()
        {
            throw new NotImplementedException();
        }

        public IConfiguredBuilder CreateNew(BuildSession session)
        {
            throw new NotImplementedException();
        }

        public IBuilder Sealed()
        {
            throw new NotImplementedException();
        }

        public int BuildDepth
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool ShouldIgnoreUnset(Type type)
        {
            throw new NotImplementedException();
        }

        public IGenerator GetGenerator(Type type, BuildSession session)
        {
            throw new NotImplementedException();
        }
    }
}

