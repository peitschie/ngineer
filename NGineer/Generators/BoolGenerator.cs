
using System;

namespace NGineer.Generators
{

    public class BoolGenerator : SimpleGenerator<bool>
	{
		private readonly Random _random;
		
		public BoolGenerator(int seed)
		{
			_random = new Random(seed);
		}

        protected override bool Generate()
		{
			return _random.Next(2) == 1;
		}
	}
}
