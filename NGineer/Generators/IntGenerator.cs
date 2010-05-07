using System;
using System.Linq;
using System.Collections.Generic;

namespace NGineer.Generators
{
    public class IntGenerator : SimpleGenerator<int>
	{
		public IntGenerator(int seed) : base(seed)
		{
		}

        protected override int Generate()
		{
			return Random.Next();
		}
	}
}
