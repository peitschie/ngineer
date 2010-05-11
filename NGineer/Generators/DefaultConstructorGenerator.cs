using System;
using System.Linq;
using NGineer.BuildHelpers;
using NGineer.Utils;

namespace NGineer.Generators
{
    public class DefaultConstructorGenerator : IGenerator
    {
        private static object InvokeDefaultConstructor(Type type)
        {
            var constructor = type.GetConstructor(new Type[0]);
            return constructor != null ? constructor.Invoke(new object[0]) : null;
        }

        public bool GeneratesType(Type type, IBuilder builder, BuildSession session)
        {
            return true;
        }

        public object Populate(object obj, IBuilder builder, BuildSession session)
        {
            var type = obj.GetType();
            foreach (var property in type.GetProperties().Where(p => p.CanWrite && !session.ConstructedMembers.Contains(p)))
            {
                property.SetValue(obj, builder.Build(property.PropertyType, session), null);
            }
            foreach (var field in type.GetFields().Where(f => f.IsPublic && !session.ConstructedMembers.Contains(f)))
            {
                field.SetValue(obj, builder.Build(field.FieldType, session));
            }
            return obj;
        }

        public object Create(Type type, IBuilder builder, BuildSession session)
        {
            object newObj = InvokeDefaultConstructor(type);
            if (newObj == null)
            {
                throw new BuilderException("Unable to construct {0} as no default constructor was found".With(type));
            }
            return newObj;
        }
    }
}