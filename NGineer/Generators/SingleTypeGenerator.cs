using System;
using System.Reflection;
using NGineer.BuildHelpers;

namespace NGineer.Generators
{
    public abstract class SingleTypeGenerator<TType> : IGenerator<TType>
    {
        public abstract TType Create(Type type, IBuilder builder, BuildSession session);

        public abstract TType Populate(TType obj, IBuilder builder, BuildSession session);

        public virtual bool GeneratesType(Type type, IBuilder builder, BuildSession session)
        {
            return typeof (TType).Equals(type);
        }

        public object Populate(Type type, object obj, IBuilder builder, BuildSession session)
        {
            return Populate((TType) obj, builder, session);
        }

        object IGenerator.Create(Type type, IBuilder builder, BuildSession session)
        {
            return Create(type, builder, session);
        }
    }
}