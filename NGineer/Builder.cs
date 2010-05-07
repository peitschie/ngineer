using System;
using System.Linq;
using System.Collections.Generic;
using NGineer.BuildHelpers;
using NGineer.Generators;

namespace NGineer
{
	public class Builder : IBuilder
	{
        protected readonly Builder Parent;
        protected readonly IList<ISetter> Setters = new List<ISetter>();

	    private readonly int _seed;
	    private readonly IList<IGenerator> _generators = new List<IGenerator>();
	    private readonly IGenerator _defaultGenerator;
		private int _maximumDepth = 20;
	    private bool _sealed;

	    public Builder(int seed)
		{
		    _defaultGenerator = new DefaultConstructorGenerator();
			WithGenerator(new ObjectGenerator(seed));
			WithGenerator(new DateTimeGenerator(seed));
			WithGenerator(new EnumGenerator(seed));
			WithGenerator(new BoolGenerator(seed));
			WithGenerator(new IntGenerator(seed));
			WithGenerator(new StringGenerator(seed));
			WithGenerator(new UIntGenerator(seed));
		}

        protected Builder(int seed, Builder parent) : this(seed)
        {
            _seed = seed;
            Parent = parent;
        }

	    public IBuilder WithGenerator(IGenerator generator)
		{
	        AssertBuilderIsntSealed();
            // each new generator should be inserted at the front of the queue to allow 
            // easy overriding of which generator to use
			_generators.Insert(0, generator);
			return this;
		}

	    private void AssertBuilderIsntSealed()
	    {
            if (_sealed)
            {
                throw new BuilderException("Builder class has been sealed.  Please use CreateNew to create a child instance");
            }
	    }

	    public IBuilder SetMaximumDepth(int depth)
		{
            AssertBuilderIsntSealed();
			_maximumDepth = depth;
			return this;
		}

        #region Set values

        public IBuilder SetValuesFor<TType>(Action<TType> setter)
        {
            AssertBuilderIsntSealed();
            Setters.Add(new Setter<TType>(setter));
            return this;
        }

        public IBuilder SetValuesFor<TType>(Func<TType, TType> setter)
        {
            AssertBuilderIsntSealed();
            Setters.Add(new Setter<TType>(setter));
            return this;
        }

	    public IBuilder SetValuesFor<TType>(Action<TType, IBuilder> setter)
	    {
            AssertBuilderIsntSealed();
            Setters.Add(new Setter<TType>(setter));
            return this;
	    }

	    public IBuilder SetValuesFor<TType>(Func<TType, IBuilder, TType> setter)
	    {
            AssertBuilderIsntSealed();
            Setters.Add(new Setter<TType>(setter));
            return this;
	    }

	    public IBuilder Seal()
	    {
	        _sealed = true;
	        return this;
	    }

	    #endregion

        public IBuilder CreateNew()
	    {
	        return new Builder(_seed, this);
	    }

	    public TType Build<TType>()
		{
			return (TType)Build(typeof(TType));
		}

	    public object Build(Type type)
		{
            return DoBuild(type, new BuildContext(_maximumDepth, this, DoBuild));
		}

        private object DoBuild(Type type, BuildContext builder)
        {
            var obj = GetGenerator(type).Generate(type, builder);
            obj = DoSetters(type, obj, builder);
            return obj;
        }
		
        private object DoSetters(Type type, object obj, BuildContext builder)
        {
            return Setters.Where(s => s.IsForType(type)).Aggregate(obj, (current, setter) => setter.Set(current, builder));
        }

        private IGenerator GetGenerator(Type type, bool returnDefault = true)
        {
            var thisGenerator = _generators.FirstOrDefault(g => g.GeneratesType(type, this));
            if(thisGenerator == null && Parent != null)
            {
                thisGenerator = Parent.GetGenerator(type, false);
            }
            return returnDefault ? (thisGenerator ?? _defaultGenerator) : thisGenerator;
        }
	}

    public class Builder<TType> : Builder, IBuilder<TType>
    {
        private readonly int _seed;

        public Builder(int seed) : base(seed)
        {
            _seed = seed;
        }

        private Builder(int seed, Builder builder) : base(seed, builder)
        {
            _seed = seed;
        }

        public new IBuilder<TType> WithGenerator(IGenerator generator)
        {
            base.WithGenerator(generator);
            return this;
        }

        public new IBuilder<TType> SetMaximumDepth(int depth)
        {
            base.SetMaximumDepth(depth);
            return this;
        }

        public new IBuilder<TType> SetValuesFor<TType1>(Action<TType1> setter)
        {
            base.SetValuesFor(setter);
            return this;
        }

        public new IBuilder<TType> SetValuesFor<TType1>(Func<TType1, TType1> setter)
        {
            base.SetValuesFor(setter);
            return this;
        }

        public new IBuilder<TType> SetValuesFor<TType1>(Action<TType1, IBuilder> setter)
        {
            base.SetValuesFor(setter);
            return this;
        }

        public new IBuilder<TType> SetValuesFor<TType1>(Func<TType1, IBuilder, TType1> setter)
        {
            base.SetValuesFor(setter);
            return this;
        }

        public new IBuilder<TType> Seal()
        {
            base.Seal();
            return this;
        }

        public new IBuilder<TType> CreateNew()
        {
            return new Builder<TType>(_seed, this);
        }

        public TType Build()
        {
            return Build<TType>();
        }

    }
}
