using System;
using NGineer.Internal;
using NGineer.Exceptions;

namespace NGineer.BuildGenerators
{
    public class DefaultConstructorGenerator : IGenerator
    {
        private readonly Type _type;

        public DefaultConstructorGenerator() { }

        public DefaultConstructorGenerator(Type type)
        {
            _type = type;
        }

        private static object InvokeDefaultConstructor(Type type)
        {
            var constructor = type.GetConstructor(Type.EmptyTypes);
            return constructor != null ? constructor.Invoke(new object[0]) : null;
        }

        public bool GeneratesType(Type type, IBuilder builder, BuildSession session)
        {
            return type != typeof(object) && (_type == null || Equals(_type, type));
        }

        public ObjectBuildRecord CreateRecord(Type type, IBuilder builder, BuildSession session)
        {
            object newObj = InvokeDefaultConstructor(type);
            if (newObj == null)
            {
                throw new BuilderException(string.Format("Unable to construct {0} as no default constructor was found", type), session);
            }
            return new ObjectBuildRecord(type, newObj, true);
        }
    }
}