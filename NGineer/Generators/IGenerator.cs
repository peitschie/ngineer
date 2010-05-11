using System;
using System.Reflection;
using NGineer.BuildHelpers;

namespace NGineer.Generators
{
	public interface IGenerator
	{
	    bool GeneratesType(Type type, IBuilder builder, BuildSession session);
        object Create(Type type, IBuilder builder, BuildSession session);
	    object Populate(object obj, IBuilder builder, BuildSession session);
	}
	
	public interface IGenerator<TType> : IGenerator
	{
        new TType Create(Type type, IBuilder builder, BuildSession session);
        TType Populate(TType obj, IBuilder builder, BuildSession session);
	}
}
