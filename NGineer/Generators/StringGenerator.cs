
using System;
using System.Text;

namespace NGineer.Generators
{

	public class StringGenerator : SimpleGenerator<string>
	{
		private readonly Random _random;
		
		public StringGenerator(int seed)
		{
			_random = new Random(seed);
		}

	    protected override string Generate()
		{
			// http://stackoverflow.com/questions/1122483/c-random-string-generator
			int size = _random.Next(40) + 10;
			var builder = new StringBuilder();
	        for (int i = 0; i < size; i++)
	        {
	            char ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * _random.NextDouble() + 65)));
	            builder.Append(ch);
	        }

	        return builder.ToString();
		}
	}
}
