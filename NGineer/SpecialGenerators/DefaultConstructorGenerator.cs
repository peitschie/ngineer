using System;
using System.Linq;
using NGineer.BuildGenerators;
using NGineer.BuildHelpers;
using NGineer.Utils;

namespace NGineer.SpecialGenerators
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

        public void Populate(Type type, object obj, IBuilder builder, BuildSession session)
        {
            foreach (var property in type.GetProperties().Where(p => p.CanWrite && !session.CurrentObject.Record.IsConstructed(p)))
            {
                session.PushMember(property);
                property.SetValue(obj, builder.Build(property.PropertyType, session), null);
				session.PopMember(true);
            }
            foreach (var field in type.GetFields().Where(f => f.IsPublic && !f.IsLiteral && !session.CurrentObject.Record.IsConstructed(f)))
            {
                session.PushMember(field);
                field.SetValue(obj, builder.Build(field.FieldType, session));
				session.PopMember(true);
            }
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