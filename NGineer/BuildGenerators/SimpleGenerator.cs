using System;
using NGineer.Internal;

namespace NGineer.BuildGenerators
{
    public abstract class SimpleGenerator<TType> : SingleTypeGenerator<TType>
    {
        protected abstract TType Generate(Random random);

        protected override TType Create(Type type, IBuilder builder, BuildSession session)
        {
            return Generate(session.Random);
        }
    }
}