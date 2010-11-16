using System;
namespace NGineer
{
    public class ObjectGenerator : IGenerator
    {
        public bool GeneratesType(Type type, IBuilder builder, BuildSession session)
        {
            return type == typeof(object);
        }

        public object Create(Type type, IBuilder builder, BuildSession session)
        {
            return null;
        }

        public void Populate(Type type, object obj, IBuilder builder, BuildSession session)
        {
        }
    }
}

