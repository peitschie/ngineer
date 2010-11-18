using System;
using NGineer.Internal;

namespace NGineer.BuildGenerators
{
    public abstract class SingleTypeGenerator<TType> : IGenerator
    {
        protected abstract TType Create(Type type, IBuilder builder, BuildSession session);

        public virtual bool GeneratesType(Type type, IBuilder builder, BuildSession session)
        {
            return typeof (TType).Equals(type);
        }

        ObjectBuildRecord IGenerator.CreateRecord(Type type, IBuilder builder, BuildSession session)
        {
            return new ObjectBuildRecord(type, Create(type, builder, session), false);
        }
    }
}