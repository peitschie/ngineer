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
			
        public ObjectBuildRecord CreateRecord(Type type, IBuilder builder, BuildSession session)
        {
            var list = new List<TType>();
            RandomExtensions.Shuffle<TType>(list, _entries(builder, session), session.Random);
            return new ObjectBuildRecord(type, list, false);
		}
	}
}
