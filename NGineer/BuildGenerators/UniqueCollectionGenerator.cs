using System;
using NGineer.BuildGenerators;
using NGineer.BuildHelpers;
using NGineer.Utils;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Linq.Expressions;

namespace NGineer.BuildGenerators
{
	public class UniqueCollectionGenerator<TType> : IGenerator
	{
		private readonly Random _random;
		private readonly Func<IBuilder, BuildSession, IEnumerable<TType>> _entries;
		
		public UniqueCollectionGenerator(int seed, IEnumerable<TType> entries)
		{
			_random = new Random(seed);
			_entries = (b, s) => entries;
		}
		
		public bool GeneratesType(Type type, IBuilder builder, BuildSession session)
		{
			return type.IsGenericType && type.IsAssignableFrom(typeof(List<TType>));
		}
			
        public object Create(Type type, IBuilder builder, BuildSession session)
		{
			return new List<TType>();
		}
		
	    public void Populate(Type type, object obj, IBuilder builder, BuildSession session)
		{
			Populate((IList<TType>)obj, builder, session);
		}
		
		protected void Populate(IList<TType> list, IBuilder builder, BuildSession session)
		{
			RandomHelpers.Shuffle<TType>(list, _entries(builder, session), _random);
		}
	}
	
	public class UniqueCollectionGenerator<TClassType, TType> : IGenerator
	{
		private readonly MemberInfo _member;
		private readonly Random _random;
		
		public UniqueCollectionGenerator(int seed, Expression<Func<TClassType, TType>> memberInfo)
		{
			_random = new Random(seed);
			_member = MemberExpressions.GetMemberInfo(memberInfo);
		}
		
		public bool GeneratesType(Type type, IBuilder builder, BuildSession session)
		{
			return type.IsGenericType && type.IsAssignableFrom(typeof(List<TClassType>));
		}
			
        public object Create(Type type, IBuilder builder, BuildSession session)
		{
			return new List<TClassType>();
		}
		
	    public void Populate(Type type, object obj, IBuilder builder, BuildSession session)
		{
			Populate((IList<TClassType>)obj, builder, session);
		}
		
		protected void Populate(IList<TClassType> list, IBuilder builder, BuildSession session)
		{
			foreach(var memberValue in RandomHelpers.Shuffle<TType>(EnumUtils.GetValues<TType>(), _random))
			{
				var entry = builder.Build<TClassType>(session);
				_member.SetValue(entry, memberValue);
				list.Add(entry);
			}
		}
	}
}
