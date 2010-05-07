using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace NGineer.Generators
{
	public interface IGenerator
	{
	    bool GeneratesType(Type type, IBuilder builder);
        object Generate(Type type, IBuilder builder);
        object Generate(PropertyInfo property, IBuilder builder);
        object Generate(FieldInfo field, IBuilder builder);
	}
	
	public interface IGenerator<out TType> : IGenerator
	{
        new TType Generate(Type type, IBuilder builder);
        new TType Generate(PropertyInfo property, IBuilder builder);
        new TType Generate(FieldInfo field, IBuilder builder);
	}
}
