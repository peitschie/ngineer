using System;
using NGineer.BuildHelpers;

namespace NGineer.BuildGenerators
{
    public abstract class SimpleGenerator<TType> : SingleTypeGenerator<TType>
    {
        protected abstract TType Generate(Random random);

        public override TType Create(Type type, IBuilder builder, BuildSession session)
        {
            return Generate(session.Random);
        }
    }
}