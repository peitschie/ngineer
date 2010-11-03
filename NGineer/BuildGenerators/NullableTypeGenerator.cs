using System;
using NGineer.BuildHelpers;
using NGineer.Utils;

namespace NGineer.BuildGenerators
{
    public class NullableTypeGenerator : IGenerator
    {
        public bool GeneratesType(Type type, IBuilder builder, BuildSession session)
        {
            return type.IsNullable();
        }

        public object Create(Type type, IBuilder builder, BuildSession session)
        {
            var nullableType = type.GetGenericArguments()[0];
            return session.Random.Next(5) == 0 ? null : builder.Build(nullableType, session);
        }

        public void Populate(Type type, object obj, IBuilder builder, BuildSession session)
        {
        }
    }
}