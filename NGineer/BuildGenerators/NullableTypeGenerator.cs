using System;
using NGineer.Internal;
using NGineer.Utils;

namespace NGineer.BuildGenerators
{
    public class NullableTypeGenerator : IGenerator
    {
        public bool GeneratesType(Type type, IBuilder builder, BuildSession session)
        {
            return type.IsNullable();
        }

        public ObjectBuildRecord CreateRecord(Type type, IBuilder builder, BuildSession session)
        {
            var nullableType = type.GetGenericArguments()[0];
            return session.Random.Next(5) == 0 ? new ObjectBuildRecord(type, null, false)
                : new ObjectBuildRecord(type, builder.Build(nullableType), false);
        }
    }
}