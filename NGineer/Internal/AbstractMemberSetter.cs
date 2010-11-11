using System;
using System.Reflection;
using NGineer.Utils;
namespace NGineer.Internal
{
	public class AbstractMemberSetter
	{
		protected readonly MemberInfo Member;
		protected readonly Type MemberReturnType;
		protected readonly Type DeclaringType;
		private readonly Func<Type, bool> _typeCheck;
		
		public AbstractMemberSetter(MemberInfo member, Type declaringType, bool allowInherited)
		{
			if (member == null)
				throw new ArgumentNullException("member");
			Member = member;
			MemberReturnType = member.ReturnType();
			DeclaringType = declaringType;
			if (allowInherited) 
			{
				_typeCheck = t => DeclaringType.IsAssignableFrom(t);
			} 
			else 
			{
				_typeCheck = t => DeclaringType == t;
			}
		}
		
		protected bool IsForMember(MemberInfo member)
		{
			return member != null 
                && _typeCheck(member.ReflectedType)
                && Equals(Member.Name, member.Name)
				&& Equals(MemberReturnType, member.ReturnType());
		}
	}
}