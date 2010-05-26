using System;
using NGineer.BuildGenerators;
using NGineer.BuildHelpers;
using System.Collections.Generic;
using System.Collections;

namespace NGineer
{
	public class UniqueCollectionGenerator<TType> : IGenerator
	{
		private readonly Random _random;
		private readonly IEnumerable<TType> _entries;
		
		public UniqueCollectionGenerator(int seed, IEnumerable<TType> entries)
		{
			_entries = entries;
			_random = new Random(seed);
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
			var entries = new List<TType>(_entries);
			while(entries.Count > 0)
			{
				var nextIndex = _random.Next(entries.Count);
				list.Add(entries[nextIndex]);
				entries.RemoveAt(nextIndex);
			}
		}
	}
}
