using System;

namespace NGineer.Generators
{

    public class BuilderGenerator<TType> : SimpleGenerator<TType>
	{
		private readonly Func<TType> _constructor;
		
		public BuilderGenerator (int seed, Func<TType> constructor) : base(seed)
		{
			_constructor = constructor;
		}

        protected override TType Generate()
		{
			return _constructor();
		}
	}
}
