using System;
using NGineer.BuildHelpers;

namespace NGineer.BuildGenerators
{
	public interface IGenerator
	{
	    bool GeneratesType(Type type, IBuilder builder, BuildSession session);
        object Create(Type type, IBuilder builder, BuildSession session);
	    void Populate(Type type, object obj, IBuilder builder, BuildSession session);
	}
	
	public interface IGenerator<TType> : IGenerator
	{
        new TType Create(Type type, IBuilder builder, BuildSession session);
        void Populate(TType obj, IBuilder builder, BuildSession session);
	}
}
