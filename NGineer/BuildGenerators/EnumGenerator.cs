using System;
using System.Reflection;
using NGineer.BuildHelpers;

namespace NGineer.BuildGenerators
{
    public class EnumGenerator : IGenerator
    {
        public bool GeneratesType(Type type, IBuilder builder, BuildSession session)
        {
            return type.IsEnum;
        }

        public object Create(Type type, IBuilder builder, BuildSession session)
        {
            return GetRandomEnum(type, session.Random);
        }

        public void Populate(Type type, object obj, IBuilder builder, BuildSession session)
        {
        }

        private object GetRandomEnum(Type type, Random random)
        {
            var array = Enum.GetValues(type);
            var index = random.Next(array.Length);
            return array.GetValue(index);
        }
    }
}