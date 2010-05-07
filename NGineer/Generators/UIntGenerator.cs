
using System;

namespace NGineer.Generators
{

    public class UIntGenerator : SimpleGenerator<uint>
	{
		public UIntGenerator(int seed) : base(seed)
		{}

        protected override uint Generate()
		{
			var nextValue = Random.Next();
			return Convert.ToUInt32(nextValue < 0 ? -nextValue : nextValue);
		}
	}
}
