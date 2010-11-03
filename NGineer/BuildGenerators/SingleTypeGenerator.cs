using System;
using NGineer.BuildHelpers;

namespace NGineer.BuildGenerators
{
    public abstract class SingleTypeGenerator<TType> : IGenerator
    {
        public abstract TType Create(Type type, IBuilder builder, BuildSession session);

        public virtual bool GeneratesType(Type type, IBuilder builder, BuildSession session)
        {
            return typeof (TType).Equals(type);
        }

        object IGenerator.Create(Type type, IBuilder builder, BuildSession session)
        {
            return Create(type, builder, session);
        }
    }
}