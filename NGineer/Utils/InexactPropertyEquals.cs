using System;
using System.Reflection;

namespace NGineer.Utils
{
	public static class InexactPropertyEquals
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
	}
}
