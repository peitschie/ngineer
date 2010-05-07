using System;
using System.Linq;

namespace NGineer
{
	public interface IBuilder
	{
		object Build(Type type);
	}
}
