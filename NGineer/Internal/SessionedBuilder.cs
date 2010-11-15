using System;
namespace NGineer.Internal
{
    public class SessionedBuilder : IBuilder
    {
        private IConfiguredBuilder _builder;
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

        public IBuilder CreateNew()
        {
            return _builder.CreateNew(_session);
        }

        public IBuilder Sealed()
        {
            return this;
        }

        public int BuildDepth {
            get { return _builder.BuildDepth; }
        }

        #region Unimplemented methods not required within a session
        public IBuilder SetMaximumDepth(int? depth)
        {
            throw new BuilderSealedException();
        }

        public IBuilder SetMaximumObjects(int? maximum)
        {
            throw new BuilderSealedException();
        }

        public IBuilder ThrowsWhenMaximumDepthReached()
        {
            throw new BuilderSealedException();
        }

        public IBuilder WithGenerator(IGenerator generator)
        {
            throw new BuilderSealedException();
        }

        public IBuilder WithGenerator(Type type, Func<IBuilder, BuildSession, object> generator)
        {
            throw new BuilderSealedException();
        }

        public IBuilder WithGenerator<TType>(Func<IBuilder, BuildSession, TType> generator)
        {
            throw new BuilderSealedException();
        }

        public IBuilder WithGenerator<TType>(Func<TType> generator)
        {
            throw new BuilderSealedException();
        }

        public IBuilder AfterConstructionOf(Internal.IMemberSetter setter)
        {
            throw new BuilderSealedException();
        }

        public IBuilder IgnoreUnset(Type type)
        {
            throw new BuilderSealedException();
        }

        public IBuilder AfterPopulationOf(IInstanceProcessor setter)
        {
            throw new BuilderSealedException();
        }

        public IBuilder SetDefaultCollectionSize(int minimum, int maximum)
        {
            throw new BuilderSealedException();
        }

        public IBuilder SetCollectionSize(Type type, int minimum, int maximum)
        {
            throw new BuilderSealedException();
        }

        public IBuilder SetCollectionSize<TType>(int minimum, int maximum)
        {
            throw new BuilderSealedException();
        }

        public IBuilder SetNumberOfInstances(Type type, int minimum, int maximum)
        {
            throw new BuilderSealedException();
        }

        public IBuilder SetNumberOfInstances<TType>(int minimum, int maximum)
        {
            throw new BuilderSealedException();
        }

        public ITypedBuilder<TType> For<TType>()
        {
            throw new BuilderSealedException();
        }

        public ITypedBuilder<TType> For<TType>(bool allowInherited)
        {
            throw new BuilderSealedException();
        }

        public IBuilder PostBuild(Action<BuildSession> hook)
        {
            throw new BuilderSealedException();
        }
        #endregion
    }
}

