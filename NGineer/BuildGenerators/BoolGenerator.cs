using System;
namespace NGineer.BuildGenerators
{

    public class BoolGenerator : SimpleGenerator<bool>
	{
        protected override bool Generate(Random random)
		{
			return random.Next(2) == 1;
		}
	}
}
