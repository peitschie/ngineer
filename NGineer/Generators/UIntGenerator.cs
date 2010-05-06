
using System;

namespace NGineer.Generators
{

	public class UIntGenerator : IGenerator<uint>
	{
		private readonly Random _random;
		
		public UIntGenerator(int seed)
		{
			_random = new Random(seed);
		}
		
		object IGenerator.Generate()
		{
			return this.Generate();
		}
		
		public uint Generate()
		{
			var nextValue = _random.Next();
			return Convert.ToUInt32(nextValue < 0 ? -nextValue : nextValue);
		}
	}
}
