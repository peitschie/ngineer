using System;
using System.Linq;
using System.Collections.Generic;

namespace NGineer.Generators
{
	public interface IGenerator
	{
		object Generate();	
	}
	
	public interface IGenerator<TType> : IGenerator
	{
		new TType Generate();
	}
}
