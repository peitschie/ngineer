using System;
using System.Reflection;
using NGineer.BuildHelpers;

namespace NGineer.Generators
{
    public class EnumGenerator : IGenerator
    {
        private readonly Random _random;

        public EnumGenerator(int seed)
        {
            _random = new Random(seed);
        }

        public bool GeneratesType(Type type, IBuilder builder, BuildSession session)
        {
            return type.IsEnum;
        }

        public object Create(Type type, IBuilder builder, BuildSession session)
        {
            return GetRandomEnum(type);
        }

        public object Populate(Type type, object obj, IBuilder builder, BuildSession session)
        {
            return obj;
        }

        private object GetRandomEnum(Type type)
        {
            var array = Enum.GetValues(type);
            var index = _random.Next(array.Length);
            return array.GetValue(index);
        }

        public object Create(PropertyInfo property, IBuilder builder, BuildSession session)
        {
            return GetRandomEnum(property.PropertyType);
        }

        public object Create(FieldInfo field, IBuilder builder, BuildSession session)
        {
            return GetRandomEnum(field.FieldType);
        }
    }
}