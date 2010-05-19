using System;
using NGineer.BuildHelpers;

namespace NGineer.BuildGenerators
{
    public class BuilderGenerator : IGenerator
    {
        private readonly Type _type;
        private readonly Func<IBuilder, BuildSession, object> _constructor;

        public BuilderGenerator(Type type, Func<IBuilder, BuildSession, object> constructor)
        {
            _type = type;
            _constructor = constructor;
        }

        public bool GeneratesType(Type type, IBuilder builder, BuildSession session)
        {
            return type.IsAssignableFrom(_type);
        }

        public virtual object Create(Type type, IBuilder builder, BuildSession session)
        {
            return _constructor(builder, session);
        }

        public void Populate(Type type, object obj, IBuilder builder, BuildSession session)
        {
        }
    }

    public class BuilderGenerator<TType> : BuilderGenerator
	{
		private readonly Func<IBuilder, BuildSession, TType> _constructor;

        public BuilderGenerator(Func<IBuilder, BuildSession, TType> constructor)
            : base(typeof(TType), null)
		{
			_constructor = constructor;
		}

        public new object Create(Type type, IBuilder builder, BuildSession session)
        {
            return _constructor(builder, session);
        }
	}
}
