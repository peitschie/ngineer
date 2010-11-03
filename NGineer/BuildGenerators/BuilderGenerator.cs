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
            return _type.Equals(type);
        }

        public virtual object Create(Type type, IBuilder builder, BuildSession session)
        {
            return _constructor(builder, session);
        }
    }

    public class BuilderGenerator<TType> : BuilderGenerator
	{
        public BuilderGenerator(Func<IBuilder, BuildSession, TType> constructor)
            : base(typeof(TType), (b, s) => constructor(b, s))
		{
		}
	}
}
