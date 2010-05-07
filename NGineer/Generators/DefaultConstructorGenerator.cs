using System;
using System.Linq;
using System.Reflection;
using NGineer.BuildHelpers;

namespace NGineer.Generators
{
    public class DefaultConstructorGenerator : SingleTypeGenerator<object>
    {
        private static object InvokeDefaultConstructor(Type type)
        {
            var constructor = type.GetConstructor(new Type[0]);
            return constructor != null ? constructor.Invoke(new object[0]) : null;
        }

        public override object Generate(FieldInfo field, IBuilder builder)
        {
            return Generate(field.FieldType, builder);
        }

        public override object Generate(Type type, IBuilder builder)
        {
            object newObj = InvokeDefaultConstructor(type);
            if (newObj == null)
            {
                throw new BuilderException("Unable to construct type " + type);
            }

            foreach (var property in type.GetProperties().Where(p => p.CanWrite))
            {
                property.SetValue(newObj, builder.Build(property.PropertyType), null);
            }
            foreach (var field in type.GetFields().Where(f => f.IsPublic))
            {
                field.SetValue(newObj, builder.Build(field.FieldType));
            }
            return newObj;
        }

        public override object Generate(PropertyInfo property, IBuilder builder)
        {
            return Generate(property.PropertyType, builder);
        }
    }
}