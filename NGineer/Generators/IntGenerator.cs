using System;
using System.Linq;
using System.Collections.Generic;

namespace NGineer.Generators
{
public class IntGenerator : IGenerator<int>
	{
		private readonly Random _random;
		
		public IntGenerator(int seed)
		{
			_random = new Random(seed);
		}
		
		object IGenerator.Generate()
		{
			return this.Generate();
		}
		
		public int Generate()
		{
			return _random.Next();
		}
	}
}
