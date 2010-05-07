using System;
using System.Reflection;

namespace NGineer.Generators
{
    public abstract class SimpleGenerator<TType> : SingleTypeGenerator<TType>
    {
        protected abstract TType Generate();

        public override TType Generate(FieldInfo field, IBuilder builder)
        {
            return Generate();
        }

        public override TType Generate(PropertyInfo property, IBuilder builder)
        {
            return Generate();
        }

        public override TType Generate(Type type, IBuilder builder)
        {
            return Generate();
        }
    }
}