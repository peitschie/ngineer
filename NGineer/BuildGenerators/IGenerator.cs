using System;
using NGineer.Internal;

namespace NGineer.BuildGenerators
{
	public interface IGenerator
	{
	    bool GeneratesType(Type type, IBuilder builder, BuildSession session);
        object Create(Type type, IBuilder builder, BuildSession session);
	    void Populate(Type type, object obj, IBuilder builder, BuildSession session);
	}
}
