
using System;

namespace NGineer.Generators
{

	public class BuilderGenerator<TType> : IGenerator<TType>
	{
		private readonly Func<TType> _constructor;
		
		public BuilderGenerator (int seed, Func<TType> constructor)
		{
			_constructor = constructor;
		}
		
		object IGenerator.Generate()
		{
			return this.Generate();
		}
		
		public TType Generate()
		{
			return _constructor();
		}
	}
}
