using System;
using System.Linq;
using System.Collections.Generic;
using NGineer.BuildHelpers;
using NGineer.Generators;

namespace NGineer
{
	public class Builder : IBuilder
	{
		private readonly IList<IGenerator> _generators = new List<IGenerator>();
        private readonly IList<ISetter> _setters = new List<ISetter>();
	    private readonly IGenerator _defaultGenerator;
		private int _maximumDepth = 20;
		
		public Builder(int seed)
		{
		    _defaultGenerator = new DefaultConstructorGenerator(seed);
			WithGenerator(new DateTimeGenerator(seed));
			WithGenerator(new EnumGenerator(seed));
			WithGenerator(new BoolGenerator(seed));
			WithGenerator(new IntGenerator(seed));
			WithGenerator(new StringGenerator(seed));
			WithGenerator(new UIntGenerator(seed));
		}

        public Builder WithGenerator(IGenerator generator)
		{
            // each new generator should be inserted at the front of the queue to allow 
            // easy overriding of which generator to use
			_generators.Insert(0, generator);
			return this;
		}

        public Builder SetMaximumDepth(int depth)
		{
			_maximumDepth = depth;
			return this;
		}

        public Builder SetValuesFor<TType>(Action<TType> setter)
        {
            _setters.Add(new Setter<TType>(WrapSetter(setter)));
            return this;
        }

        public Builder SetValuesFor<TType>(Func<TType, TType> setter)
        {
            _setters.Add(new Setter<TType>(setter));
            return this;
        }
		
		public TType Build<TType>()
		{
			return (TType)Build(typeof(TType));
		}
		
		public object Build(Type type)
		{
            return DoBuild(type, new BuildContext(_maximumDepth, DoBuild));
		}

        private object DoBuild(Type type, BuildContext builder)
        {
            var obj = GetGenerator(type).Generate(type, builder);
            obj = DoSetters(type, obj);
            return obj;
        }
		
        private object DoSetters(Type type, object obj)
        {
            return _setters.Where(s => s.IsForType(type)).Aggregate(obj, (current, setter) => setter.Set(current));
        }

	    private static Func<TType, TType> WrapSetter<TType>(Action<TType> setter)
        {
            return (s) =>
                       {
                           setter.Invoke(s);
                           return s;
                       };
        }

        private IGenerator GetGenerator(Type type)
        {
            return _generators.FirstOrDefault(g => g.GeneratesType(type, this)) ?? _defaultGenerator;
        }
	}
}
