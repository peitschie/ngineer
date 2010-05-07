using System;
using System.Linq;
using System.Collections.Generic;

namespace NGineer.Generators
{
    public class IntGenerator : SimpleGenerator<int>
	{
		private readonly Random _random;
		
		public IntGenerator(int seed)
		{
			_random = new Random(seed);
		}

        protected override int Generate()
		{
			return _random.Next();
		}
	}
}
