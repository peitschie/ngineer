
using System;

namespace NGineer.Generators
{

    public class UIntGenerator : SimpleGenerator<uint>
	{
		private readonly Random _random;
		
		public UIntGenerator(int seed)
		{
			_random = new Random(seed);
		}

        protected override uint Generate()
		{
			var nextValue = _random.Next();
			return Convert.ToUInt32(nextValue < 0 ? -nextValue : nextValue);
		}
	}
}
