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
            var previousMember = session.CurrentMember;
            foreach (var property in type.GetProperties().Where(p => p.CanWrite && !session.CurrentObject.Record.IsConstructed(p)))
            {
                session.CurrentMember = property;
                property.SetValue(obj, builder.Build(property.PropertyType, session), null);
            }
            foreach (var field in type.GetFields().Where(f => f.IsPublic && !f.IsLiteral && !session.CurrentObject.Record.IsConstructed(f)))
            {
                session.CurrentMember = field;
                field.SetValue(obj, builder.Build(field.FieldType, session));
            }
            session.CurrentMember = previousMember;
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