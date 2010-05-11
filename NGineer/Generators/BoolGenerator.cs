using System;

namespace NGineer.Generators
{

    public class BoolGenerator : SimpleGenerator<bool>
	{
        public BoolGenerator(int seed) : base(seed)
        {
        }

        protected override bool Generate()
		{
			return Random.Next(2) == 1;
		}
	}
}
