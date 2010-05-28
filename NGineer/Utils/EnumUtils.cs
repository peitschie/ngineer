using System;
using System.Collections.Generic;

namespace NGineer.Utils
{
	public static class EnumUtils
	{
		public static IEnumerable<TEnumType> GetValues<TEnumType>()
		{
			var arr = Enum.GetValues(typeof(TEnumType));
			var list = new List<TEnumType>(arr.Length);
			foreach(var val in arr)
			{
				list.Add((TEnumType)val);
			}
			return list;
		}
	}
}
