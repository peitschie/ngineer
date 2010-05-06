
using System;

namespace NGineer.Generators
{

	public class BoolGenerator : IGenerator<bool>
	{
		private readonly Random _random;
		
		public BoolGenerator(int seed)
		{
			_random = new Random(seed);
		}
		
		object IGenerator.Generate()
		{
			return this.Generate();
		}
		
		public bool Generate()
		{
			return _random.Next(2) == 1;
		}
	}
}
