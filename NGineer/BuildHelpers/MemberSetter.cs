using System.Reflection;
using NGineer.Utils;
using System;

namespace NGineer.BuildHelpers
{
    public class MemberSetter<TObjType, TReturnType> : IMemberSetter
    {
		protected readonly MemberInfo Member;
        protected readonly Func<TObjType, IBuilder, BuildSession, TReturnType> Setter;

        public MemberSetter(MemberInfo member, Func<TObjType, IBuilder, BuildSession, TReturnType> setter)
        {
			if(member == null)
				throw new ArgumentNullException("member");
			if(setter == null)
				throw new ArgumentNullException("setter");
            Member = member;
            Setter = setter;
        }

        public bool IsForMember(MemberInfo member, IBuilder builder, BuildSession session)
        {
            return member != null 
                && Member.ReflectedType.IsAssignableFrom(member.ReflectedType)
                && Equals(Member.DeclaringType, member.DeclaringType)
                && Equals(Member.Name, member.Name)
				&& Equals(Member.ReturnType(), member.ReturnType());
        }

        public void Set(object obj, IBuilder builder, BuildSession session)
		{
			Member.SetValue(obj, Setter((TObjType)obj, builder, session));
		}
    }
	
	public class MemberSetter : MemberSetter<object, object>
	{
		public MemberSetter(MemberInfo member, Func<object, IBuilder, BuildSession, object> setter)
			: base(member, setter)
        {}
	}
}