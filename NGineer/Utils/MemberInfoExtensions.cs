using System;
using System.Reflection;

namespace NGineer.Utils
{
	public static class MemberInfoExtensions
	{
		public static bool LikelyEqual(this MemberInfo self, MemberInfo other)
		{
			return other != null
				&& self.Name == other.Name
				&& self.MemberType == other.MemberType
				&& self.DeclaringType == other.DeclaringType
				&& self.MemberType == MemberTypes.Property ? 
					LikelyEqual(self as PropertyInfo, other as PropertyInfo)
					: LikelyEqual(self as FieldInfo, other as FieldInfo);
		}
		
		private static bool LikelyEqual(this PropertyInfo self, PropertyInfo other)
		{
			return other != null
				&& self.PropertyType == other.PropertyType;
		}
		
		private static bool LikelyEqual(this FieldInfo self, FieldInfo other)
		{
			return other != null
				&& self.FieldType == other.FieldType;
		}

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
	}
}
