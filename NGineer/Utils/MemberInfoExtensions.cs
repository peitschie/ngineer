using System;
using System.Reflection;

namespace NGineer.Utils
{
	public static class MemberInfoExtensions
	{
        public static Type ReturnType(this MemberInfo self)
        {
            if (self.MemberType == MemberTypes.Field)
                return ((FieldInfo) self).FieldType;
            if (self.MemberType == MemberTypes.Property)
                return ((PropertyInfo)self).PropertyType;
            throw new ArgumentException("Unsupported member type {0}".With(self.MemberType));
        }
		
		public static void SetValue(this MemberInfo self, object obj, object value)
        {
            if (self.MemberType == MemberTypes.Field)
                ((FieldInfo) self).SetValue(obj, value);
            else if (self.MemberType == MemberTypes.Property)
                ((PropertyInfo)self).SetValue(obj, value, null);
			else
            	throw new ArgumentException("Unsupported member type {0}".With(self.MemberType));
        }

        public static bool IsNullable(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
	}
}
