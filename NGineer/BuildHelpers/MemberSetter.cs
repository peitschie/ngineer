using System.Reflection;
using NGineer.Utils;
using System;

namespace NGineer.BuildHelpers
{
    public class MemberSetter<TObjType, TReturnType> : IMemberSetter
    {
		protected readonly MemberInfo Member;
        protected readonly Type MemberReturnType;

        protected readonly Func<TObjType, IBuilder, BuildSession, TReturnType> Setter;

        protected MemberSetter(MemberInfo member)
        {
            if(member == null)
                throw new ArgumentNullException("member");
            Member = member;
            MemberReturnType = member.ReturnType();
        }

        public MemberSetter(MemberInfo member, Func<TObjType, IBuilder, BuildSession, TReturnType> setter)
            : this(member)
        {
			if(setter == null)
				throw new ArgumentNullException("setter");
            Setter = setter;
        }

        public virtual bool IsForMember(MemberInfo member, IBuilder builder, BuildSession session)
        {
            return member != null 
                && Member.ReflectedType.IsAssignableFrom(member.ReflectedType)
                && Equals(Member.DeclaringType, member.DeclaringType)
                && Equals(Member.Name, member.Name)
				&& Equals(MemberReturnType, member.ReturnType());
        }

        public virtual void Set(object obj, IBuilder builder, BuildSession session)
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