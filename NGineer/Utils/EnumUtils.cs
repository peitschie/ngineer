using System;
using System.Collections.Generic;
using System.Linq;

namespace NGineer.Utils
{
	public static class EnumUtils
	{
        public static object[] GetValues(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Type '" + enumType.Name + "' is not an enum");
            }

            var fields = from field in enumType.GetFields()
                         where field.IsLiteral
                         select field;

            return fields.Select(field => field.GetValue(enumType)).ToArray();
        }

		public static IEnumerable<TEnumType> GetValues<TEnumType>()
		{
			var arr = GetValues(typeof(TEnumType));
			var list = new List<TEnumType>(arr.Length);
			foreach(var val in arr)
			{
				list.Add((TEnumType)val);
			}
			return list;
		}
	}
}
