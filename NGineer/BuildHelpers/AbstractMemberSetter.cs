using System;
using System.Reflection;
using NGineer.Utils;
namespace NGineer.BuildHelpers
{
	public class AbstractMemberSetter
	{
		protected readonly MemberInfo Member;
		protected readonly Type MemberReturnType;
		private readonly Func<Type, bool> _typeCheck;
		
		public AbstractMemberSetter(MemberInfo member, bool allowInherited)
		{
			if (member == null)
				throw new ArgumentNullException("member");
			Member = member;
			MemberReturnType = member.ReturnType();
			if (allowInherited) 
			{
				_typeCheck = t => member.ReflectedType.IsAssignableFrom(t);
			} 
			else 
			{
				_typeCheck = t => member.ReflectedType == t;
			}
		}
		
		protected bool IsForMember(MemberInfo member)
		{
			return member != null 
                && _typeCheck(member.ReflectedType)
                && Equals(Member.DeclaringType, member.DeclaringType)
                && Equals(Member.Name, member.Name)
				&& Equals(MemberReturnType, member.ReturnType());
		}
	}
}