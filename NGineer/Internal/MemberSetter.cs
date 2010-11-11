using System.Reflection;
using NGineer.Utils;
using System;

namespace NGineer.Internal
{
    public class MemberSetter<TObjType, TReturnType> : AbstractMemberSetter, IMemberSetter
    {
        private readonly Func<TObjType, IBuilder, BuildSession, TReturnType> _setter;

        public MemberSetter(MemberInfo member, Func<TObjType, IBuilder, BuildSession, TReturnType> setter, 
			bool allowInherited)
            : base(member, typeof(TObjType), allowInherited)
        {
        	if (setter == null)
        		throw new ArgumentNullException("setter");
        	_setter = setter;
        }

        public bool IsForMember(MemberInfo member, IBuilder builder, BuildSession session)
        {
            return IsForMember(member);
        }

        public void Set(object obj, IBuilder builder, BuildSession session)
		{
			Member.SetValue(obj, _setter((TObjType)obj, builder, session));
		}
    }
}