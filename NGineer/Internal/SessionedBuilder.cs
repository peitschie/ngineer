using System;
namespace NGineer
{
    public class SessionedBuilder : IBuilder
    {
        private IBuilder _builder;
        private BuildSession _session;

        public SessionedBuilder(IConfiguredBuilder builder, BuildSession session)
        {
            _builder = builder;
            _session = session;
        }

        public object Build(Type type)
        {
            return _session.Build(type);
        }

        public TType Build<TType>()
        {
            return _session.Build<TType>();
        }

        public TType Build<TType>(BuildSession session)
        {
            return _session.Build<TType>();
        }

        public object Build(Type type, BuildSession session)
        {
            return _session.Build(type);
        }

        public IBuilder SetMaximumDepth(int? depth)
        {
            throw new NotImplementedException();
        }

        public IBuilder SetMaximumObjects(int? maximum)
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

        public IBuilder AfterConstructionOf(Internal.IMemberSetter setter)
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
            return _builder.CreateNew();
        }

        public IBuilder Sealed()
        {
            return this;
        }

        public int BuildDepth
        {
            get
            {
                return _builder.BuildDepth;
            }
        }
    }
}

