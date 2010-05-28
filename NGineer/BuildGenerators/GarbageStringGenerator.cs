using System;
using System.Text;

namespace NGineer.BuildGenerators
{

	public class GarbageStringGenerator : SimpleGenerator<string>
	{
	    protected override string Generate(Random random)
		{
			// http://stackoverflow.com/questions/1122483/c-random-string-generator
			int size = random.Next(40) + 10;
			var builder = new StringBuilder();
	        for (int i = 0; i < size; i++)
	        {
	            char ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
	            builder.Append(ch);
	        }

	        return builder.ToString();
		}
	}
}
