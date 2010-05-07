using System;
using System.Reflection;

namespace NGineer.Generators
{
    public class EnumGenerator : IGenerator
    {
        private readonly Random _random;

        public EnumGenerator(int seed)
        {
            _random = new Random(seed);
        }

        public bool GeneratesType(Type type, IBuilder builder)
        {
            return type.IsEnum;
        }

        public object Generate(Type type, IBuilder builder)
        {
            return GetRandomEnum(type);
        }

        private object GetRandomEnum(Type type)
        {
            var array = Enum.GetValues(type);
            var index = _random.Next(array.Length);
            return array.GetValue(index);
        }

        public object Generate(PropertyInfo property, IBuilder builder)
        {
            return GetRandomEnum(property.PropertyType);
        }

        public object Generate(FieldInfo field, IBuilder builder)
        {
            return GetRandomEnum(field.FieldType);
        }
    }
}