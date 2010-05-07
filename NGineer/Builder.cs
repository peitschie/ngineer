using System;
using System.Linq;
using System.Collections.Generic;
using NGineer.Generators;
using NGineer.Utils;

namespace NGineer
{
	public class Builder : IBuilder
	{
		private readonly IList<IGenerator> _generators = new List<IGenerator>();
	    private readonly IGenerator _defaultGenerator;
		private int _maximumDepth = 20;
		
		public Builder(int seed)
		{
		    _defaultGenerator = new DefaultConstructorGenerator(seed);
			WithGenerator(new IntGenerator(seed));
			WithGenerator(new StringGenerator(seed));
			WithGenerator(new UIntGenerator(seed));
		}

        public Builder WithGenerator(IGenerator generator)
		{
			_generators.Add(generator);
			return this;
		}

        public Builder SetMaximumDepth(int depth)
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
            return DoBuild(type, new BuildContext(_maximumDepth, DoBuild));
		}

        private object DoBuild(Type type, BuildContext builder)
        {
            return GetGenerator(type).Generate(type, builder);
        }
		
        private IGenerator GetGenerator(Type type)
        {
            return _generators.FirstOrDefault(g => g.GeneratesType(type, this)) ?? _defaultGenerator;
        }
	}

    public class BuildContext : IBuilder
    {
        private readonly int _maximumDepth;
        private readonly Func<Type, BuildContext, object> _buildAction;
        private int _currentLevel;

        public BuildContext(int maximumDepth, Func<Type, BuildContext, object> buildAction)
        {
            _maximumDepth = maximumDepth;
            _buildAction = buildAction;
        }

        public object Build(Type type)
        {
            if (_currentLevel == _maximumDepth)
                return null;

            _currentLevel++;
            var obj = _buildAction(type, this);
            _currentLevel--;
            return obj;
        }
    }

    public class BuilderException : Exception
    {
        public BuilderException(string s) : base(s)
        {
            
        }
    }
}
