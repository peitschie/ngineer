using System;
using System.Linq;
using System.Collections.Generic;
using NGineer.BuildHelpers;
using NGineer.Generators;

namespace NGineer
{
	public abstract class BaseBuilder : IBuilder
	{
		public abstract object DoBuild(Type type, BuildSession session);
		public abstract object Build(Type type);
        public abstract IBuilder WithGenerator(IGenerator generator);
        public abstract IBuilder SetMaximumDepth(int depth);
        
		public abstract IBuilder CreateNew();
		public abstract IBuilder CreateNew(BuildSession session);
		
        public abstract TType Build<TType>();

        public abstract IBuilder SetValuesFor<TType>(Action<TType> setter);
        public abstract IBuilder SetValuesFor<TType>(Func<TType, TType> setter);
        public abstract IBuilder SetValuesFor<TType>(Action<TType, IBuilder> setter);
        public abstract IBuilder SetValuesFor<TType>(Func<TType, IBuilder, TType> setter);
		
		public abstract IBuilder SetCollectionSize(int minimum, int maximum);

        /// <summary>
        /// Marks this builder as sealed.  This prevents accidently overriding values, depths or generators.
        /// It is highly recommended that all builders be sealed after initial creation to prevent later tests
        /// from modifying the container behaviour
        /// </summary>
        /// <returns></returns>
        public abstract IBuilder Sealed();
	}
	
	public class Builder : BaseBuilder
	{
        protected readonly Builder Parent;
        protected readonly IList<ISetter> Setters = new List<ISetter>();

	    protected readonly int Seed;
	    private readonly IList<IGenerator> _generators = new List<IGenerator>();
	    private readonly IGenerator _defaultGenerator;
		private readonly ListGenerator _defaultListGenerator;
		private int _maximumDepth = 20;
	    private bool _sealed;
		private BuildSession _session;

	    public Builder(int seed)
		{
			Seed = seed;
		    _defaultGenerator = new DefaultConstructorGenerator();
			_defaultListGenerator = new ListGenerator(seed, 10, 20);
			WithGenerator(_defaultListGenerator);
			WithGenerator(new ObjectGenerator(seed));
			WithGenerator(new DateTimeGenerator(seed));
			WithGenerator(new EnumGenerator(seed));
			WithGenerator(new BoolGenerator(seed));
			WithGenerator(new IntGenerator(seed));
			WithGenerator(new StringGenerator(seed));
			WithGenerator(new UIntGenerator(seed));
		}
		
        protected Builder(Builder parent, BuildSession session) : this(parent.Seed)
        {
			_session = session;
            Parent = parent;
        }

	    public override IBuilder WithGenerator(IGenerator generator)
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

	    public override IBuilder SetMaximumDepth(int depth)
		{
            AssertBuilderIsntSealed();
			_maximumDepth = depth;
			return this;
		}

		public override IBuilder SetCollectionSize(int minimum, int maximum)
		{
			AssertBuilderIsntSealed();
			_defaultListGenerator.MinimumListItems = minimum;
			_defaultListGenerator.MaximumListItems = maximum;
			return this;	
		}
		
        #region Set values

        public override IBuilder SetValuesFor<TType>(Action<TType> setter)
        {
            AssertBuilderIsntSealed();
            Setters.Add(new Setter<TType>(setter));
            return this;
        }

        public override IBuilder SetValuesFor<TType>(Func<TType, TType> setter)
        {
            AssertBuilderIsntSealed();
            Setters.Add(new Setter<TType>(setter));
            return this;
        }

	    public override IBuilder SetValuesFor<TType>(Action<TType, IBuilder> setter)
	    {
            AssertBuilderIsntSealed();
            Setters.Add(new Setter<TType>(setter));
            return this;
	    }

	    public override IBuilder SetValuesFor<TType>(Func<TType, IBuilder, TType> setter)
	    {
            AssertBuilderIsntSealed();
            Setters.Add(new Setter<TType>(setter));
            return this;
	    }

	    #endregion
		
	    public override IBuilder Sealed()
	    {
	        _sealed = true;
	        return this;
	    }		
		
        public override IBuilder CreateNew()
	    {
	        return new Builder(this, _session);
	    }
		
		public override IBuilder CreateNew(BuildSession session)
	    {
	        return new Builder(this, session);
	    }
		
	    public override TType Build<TType>()
		{
			return (TType)Build(typeof(TType));
		}

	    public override object Build(Type type)
		{
			if(_session == null || _session.Disposed)
			{
				_session = null;
				using(var session = new BuildSession(this))
				{
	            	return DoBuild(type, session);
				}
			} 
			else 
			{
				return DoBuild(type, _session);
			}
		}

        public override object DoBuild(Type type, BuildSession session)
        {
			if(session.BuildDepth > _maximumDepth)
				return null;
			
            var obj = GetGeneratorOrDefault(type).Generate(type, session);
            obj = DoSetters(type, obj, session);
            return obj;
        }
		
        private object DoSetters(Type type, object obj, BuildSession builder)
        {
			var setters = Setters.Where(s => s.IsForType(type)).ToArray();
            return setters.Aggregate(obj, (current, setter) => setter.Set(current, builder));
        }

		private IGenerator GetGeneratorOrDefault(Type type)
        {
            var thisGenerator = GetGenerator(type);
            return thisGenerator == null ? _defaultGenerator : thisGenerator;
        }
		
        private IGenerator GetGenerator(Type type)
        {
            var thisGenerator = _generators.FirstOrDefault(g => g.GeneratesType(type, this));
            if(thisGenerator == null && Parent != null)
            {
                thisGenerator = Parent.GetGenerator(type);
            }
            return thisGenerator;
        }
	}

    public class Builder<TType> : Builder, IBuilder<TType>
    {
        public Builder(int seed) : base(seed)
        {}

        protected Builder(Builder builder) : base(builder, null)
        {}

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
		
		public new IBuilder<TType> SetCollectionSize(int minimum, int maximum)
		{
			base.SetCollectionSize(minimum, maximum);
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
		
        public new IBuilder<TType> Sealed()
        {
            base.Sealed();
            return this;
        }

        public new IBuilder<TType> CreateNew()
        {
            return new Builder<TType>(this);
        }
		
        public TType Build()
        {
            return Build<TType>();
        }

    }
}
