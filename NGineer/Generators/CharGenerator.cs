namespace NGineer.Generators
{

    public class CharGenerator : SimpleGenerator<char>
	{
        public CharGenerator(int seed) : base(seed)
        {
        }

        protected override char Generate()
		{
			return (char)Random.Next();
		}
	}
}
