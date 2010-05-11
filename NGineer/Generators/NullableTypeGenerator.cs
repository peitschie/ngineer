using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NGineer.BuildHelpers;
using NGineer.Utils;

namespace NGineer.Generators
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
            return null;
        }

        public object Populate(Type type, object obj, IBuilder builder, BuildSession session)
        {
            var nullableType = type.GetGenericArguments()[0];
            return _random.Next(5) == 0 ? null : builder.Build(nullableType, session);
        }
    }
}