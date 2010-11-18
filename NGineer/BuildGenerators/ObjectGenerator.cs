using System;
using NGineer.Internal;
namespace NGineer
{
    public class ObjectGenerator : IGenerator
    {
        public bool GeneratesType(Type type, IBuilder builder, BuildSession session)
        {
            return type == typeof(object);
        }

        public ObjectBuildRecord CreateRecord(Type type, IBuilder builder, BuildSession session)
        {
            return new ObjectBuildRecord(type, null, false);
        }
    }
}

