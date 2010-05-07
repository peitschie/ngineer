using System;
using System.Reflection;

namespace NGineer.Generators
{
    public abstract class SingleTypeGenerator<TType> : IGenerator<TType>
    {
        public bool GeneratesType(Type type, IBuilder builder)
        {
            return typeof (TType).Equals(type);
        }

        public abstract TType Generate(Type type, IBuilder builder);

        public abstract TType Generate(PropertyInfo property, IBuilder builder);

        public abstract TType Generate(FieldInfo field, IBuilder builder);

        object IGenerator.Generate(Type type, IBuilder builder)
        {
            return Generate(type, builder);
        }

        object IGenerator.Generate(PropertyInfo property, IBuilder builder)
        {
            return Generate(property, builder);
        }

        object IGenerator.Generate(FieldInfo field, IBuilder builder)
        {
            return Generate(field, builder);
        }
    }
}