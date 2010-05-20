using System;
using NGineer.BuildHelpers;
using NGineer.Utils;

namespace NGineer.BuildGenerators
{
    public class NullableTypeGenerator : IGenerator
    {
        private readonly Random _random;

        public NullableTypeGenerator(int seed)
        {
            _random = new Random(seed);
        }

        public bool GeneratesType(Type type, IBuilder builder, BuildSession session)
        {
            return typeof (Nullable<>).IsGenericAssignableFrom(type);
        }

        public object Create(Type type, IBuilder builder, BuildSession session)
        {
            var nullableType = type.GetGenericArguments()[0];
            return _random.Next(5) == 0 ? null : builder.Build(nullableType, session);
        }

        public void Populate(Type type, object obj, IBuilder builder, BuildSession session)
        {
        }
    }
}