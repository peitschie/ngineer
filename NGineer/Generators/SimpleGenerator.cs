using System;
using System.Reflection;
using NGineer.BuildHelpers;

namespace NGineer.Generators
{
    public abstract class SimpleGenerator<TType> : SingleTypeGenerator<TType>
    {
        protected readonly Random Random;

        protected SimpleGenerator(int seed)
        {
            Random = new Random(seed);
        }

        protected abstract TType Generate();

        public override TType Populate(TType obj, IBuilder builder, BuildSession session)
        {
            return obj;
        }

        public override TType Create(Type type, IBuilder builder, BuildSession session)
        {
            return Generate();
        }
    }
}