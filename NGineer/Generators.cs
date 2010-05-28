using NGineer.BuildGenerators;
using NGineer.Utils;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;

namespace NGineer
{
    public static class Generators
    {
        public static IGenerator NullGenerator<TType>() where TType : class
        {
            return new NullObjectGenerator<TType>();
        }
		
		public static IGenerator UniqueCollection<TType>()
		{
			return new UniqueCollectionGenerator<TType>(EnumUtils.GetValues<TType>());
		}
		
		public static IGenerator UniqueCollection<TType>(IEnumerable<TType> entries)
		{
			return new UniqueCollectionGenerator<TType>(entries);
		}
		
		public static IGenerator UniqueCollection<TClassType, TType>(Expression<Func<TClassType, TType>> entries)
		{
			return new UniqueCollectionGenerator<TClassType, TType>(entries);
		}
    }
}