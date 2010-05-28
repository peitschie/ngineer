using System;
namespace NGineer.BuildGenerators
{

    public class CharGenerator : SimpleGenerator<char>
	{
        protected override char Generate(Random random)
		{
			return (char)random.Next();
		}
	}
}
