using System;
using NGineer.Internal;

namespace NGineer
{
	public interface IGenerator
	{
	    bool GeneratesType(Type type, IBuilder builder, BuildSession session);
        ObjectBuildRecord CreateRecord(Type type, IBuilder builder, BuildSession session);
	}
}
