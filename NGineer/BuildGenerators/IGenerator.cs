using System;
using NGineer.BuildHelpers;

namespace NGineer.BuildGenerators
{
	public interface IGenerator
	{
	    bool GeneratesType(Type type, IBuilder builder, BuildSession session);
        object Create(Type type, IBuilder builder, BuildSession session);
	}
}
