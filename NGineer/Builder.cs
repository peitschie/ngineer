using System;
using System.Linq;
using System.Collections.Generic;
using NGineer.Generators;

namespace NGineer
{
	public class Builder : IBuilder
	{
		private readonly Dictionary<Type, IGenerator> _generators = new Dictionary<Type, IGenerator>();
		private int _maximumDepth = 20;
		
		
		public Builder(int seed)
		{
			WithGenerator(new IntGenerator(seed));
			WithGenerator(new StringGenerator(seed));
		}
		
		public IBuilder WithGenerator<TType>(IGenerator<TType> generator)
		{
			_generators.Add(typeof(TType), generator);
			return this;
		}
		
		public IBuilder SetMaximumDepth(int depth)
		{
			_maximumDepth = depth;
			return this;
		}
		
		public TType Build<TType>()
		{
			return (TType)Build(typeof(TType));
		}
		
		public object Build(Type type)
		{
			return DoBuild(type, 0);
		}
		
		private object DoBuild(Type type, int level)
		{
			object newObj = null;
			if(level <= _maximumDepth)
			{
				if(_generators.ContainsKey(type))
				{
					newObj = _generators[type].Generate();
				}
				else
				{
					var constructors = type.GetConstructors();
					foreach(var constructor in constructors)
					{
						if(constructor.GetParameters().Length == 0)
						{
							newObj = constructor.Invoke(new object[0]);
						}
					}
					if(newObj != null)
					{
						foreach(var property in type.GetProperties().Where(p => p.CanWrite))
						{
							property.SetValue(newObj, DoBuild(property.PropertyType, level+1), null);
						}
					}
				}
			}
			return newObj;
		}
	}
}
