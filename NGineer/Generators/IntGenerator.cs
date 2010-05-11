using System;
using System.Linq;
using System.Collections.Generic;
using NGineer.BuildHelpers;

namespace NGineer.Generators
{
    public class IntGenerator : IGenerator
	{
        private static readonly Type[] Types = new[] {typeof (int), typeof (Int32)};
        private readonly Random _random;
        

		public IntGenerator(int seed)
		{
            _random = new Random(seed);
		}
        
        public bool GeneratesType(Type type, IBuilder builder, BuildSession session)
        {
            return Types.Contains(type);
        }

        public object Create(Type type, IBuilder builder, BuildSession session)
        {
            return _random.Next();
        }

        public object Populate(Type type, object obj, IBuilder builder, BuildSession session)
        {
            return obj;
        }
	}
}
