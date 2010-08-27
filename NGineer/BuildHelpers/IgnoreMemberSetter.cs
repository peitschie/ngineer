using System;
using System.Reflection;

namespace NGineer.BuildHelpers
{
    public class IgnoreMemberSetter : AbstractMemberSetter, IMemberSetter
    {
        public IgnoreMemberSetter(MemberInfo member, Type declaringType, bool allowInherited)
			: base(member, declaringType, allowInherited)
        {
        }
		
		public bool IsForMember(MemberInfo member, IBuilder builder, BuildSession session)
		{
			return IsForMember(member);
		}

        public void Set(object obj, IBuilder builder, BuildSession session)
        {
        }
    }
}