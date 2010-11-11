using System;
using NGineer.BuildGenerators;
using NGineer.Internal;
using NGineer.Utils;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Linq.Expressions;

namespace NGineer.BuildGenerators
{
    public class UniqueCollectionGeneratorEnumerable<TType> : IGenerator
	{
		private readonly Func<IBuilder, BuildSession, IEnumerable<TType>> _entries;
		
		public UniqueCollectionGeneratorEnumerable(IEnumerable<TType> entries)
		{
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
			RandomHelpers.Shuffle<TType>(list, _entries(builder, session), session.Random);
		}
	}
}
