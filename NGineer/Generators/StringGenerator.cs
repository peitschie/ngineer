
using System;
using System.Text;

namespace NGineer.Generators
{

	public class StringGenerator : SimpleGenerator<string>
	{
		public StringGenerator(int seed) : base(seed)
		{}

	    protected override string Generate()
		{
			// http://stackoverflow.com/questions/1122483/c-random-string-generator
			int size = Random.Next(40) + 10;
			var builder = new StringBuilder();
	        for (int i = 0; i < size; i++)
	        {
	            char ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * Random.NextDouble() + 65)));
	            builder.Append(ch);
	        }

	        return builder.ToString();
		}
	}
}
