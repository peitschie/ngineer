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
		
		public static IGenerator UniqueCollection<TType>(int seed)
		{
			return new UniqueCollectionGenerator<TType>(seed, EnumUtils.GetValues<TType>());
		}
		
		public static IGenerator UniqueCollection<TType>(int seed, IEnumerable<TType> entries)
		{
			return new UniqueCollectionGenerator<TType>(seed, entries);
		}
		
		public static IGenerator UniqueCollection<TClassType, TType>(int seed, Expression<Func<TClassType, TType>> entries)
		{
			return new UniqueCollectionGenerator<TClassType, TType>(seed, entries);
		}
    }
}