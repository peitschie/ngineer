using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using NGineer.Utils;
using NGineer.BuildHelpers;
using NGineer;

namespace NGineer.BuildGenerators
{
	public class MemberNamingGenerator : IGenerator
	{
		private readonly MemberInfo[] _members;
		private readonly Dictionary<MemberInfo, int> _lastCount = new Dictionary<MemberInfo, int>();
		
		public MemberNamingGenerator(MemberInfo[] members)
		{
			foreach(var member in members)
			{
				Type type;
				switch(member.MemberType)
				{
					case MemberTypes.Property:
						type = ((PropertyInfo)member).PropertyType;	
						break;
					case MemberTypes.Field:
						type = ((FieldInfo)member).FieldType;
						break;
					default:
						throw new ArgumentOutOfRangeException(member.MemberType.ToString());
				}
				if(!type.Equals(typeof(string)))
				{
					throw new ArgumentException("Member {0} return type should be string but was {1}".With(member, type));
				}
				_lastCount.Add(member, 0);
			}
			_members = members;
		}
		
		public bool GeneratesType(Type type, IBuilder builder, BuildSession session)
		{
			return typeof(string).Equals(type) 
				&& _members.FirstOrDefault(m => m.LikelyEqual(session.CurrentMember)) != null;
		}
		
        public object Create(Type type, IBuilder builder, BuildSession session)
		{
			var memberInfo = _lastCount.First(m => m.Key.LikelyEqual(session.CurrentMember));
			var count = memberInfo.Value;
			_lastCount[memberInfo.Key] = count + 1;
			return session.CurrentMember.Name + count;
		}
		
	    public void Populate(Type type, object obj, IBuilder builder, BuildSession session)
		{
		}
	}
}
