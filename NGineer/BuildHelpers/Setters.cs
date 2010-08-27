
using System.Reflection;
using NGineer.Utils;
using System;
namespace NGineer.BuildHelpers
{
	public static class Setters
	{
		public static MemberSetter<TObjType, TReturnType> GetMemberSetter<TObjType, TReturnType>(MemberInfo member, 
			Func<TObjType, IBuilder, BuildSession, TReturnType> setter, bool allowInherited)
		{
			return new MemberSetter<TObjType, TReturnType>(member, setter, allowInherited);
		}
		
		public static Setter<TObjType> GetSetter<TObjType>(Func<TObjType, IBuilder, BuildSession, TObjType> setter, 
			bool allowInherited)
		{
			return new Setter<TObjType>(setter, allowInherited);
		}
	}
}
