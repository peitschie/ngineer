using System;
using NGineer.BuildHelpers;

namespace NGineer.BuildGenerators
{
    public abstract class SingleTypeGenerator<TType> : IGenerator<TType>
    {
        public abstract TType Create(Type type, IBuilder builder, BuildSession session);

        public abstract void Populate(TType obj, IBuilder builder, BuildSession session);

        public virtual bool GeneratesType(Type type, IBuilder builder, BuildSession session)
        {
            return typeof (TType).Equals(type);
        }

        public void Populate(Type type, object obj, IBuilder builder, BuildSession session)
        {
            Populate((TType) obj, builder, session);
        }

        object IGenerator.Create(Type type, IBuilder builder, BuildSession session)
        {
            return Create(type, builder, session);
        }
    }
}