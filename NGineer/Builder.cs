using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using NGineer.BuildHelpers;
using NGineer.Generators;
using NGineer.Utils;

namespace NGineer
{
	public class Builder : IBuilder
	{
        protected readonly Builder Parent;
        protected readonly IList<ISetter> Setters = new List<ISetter>();
        protected readonly IList<IMemberSetter> MemberSetters = new List<IMemberSetter>();

	    protected readonly int Seed;
	    private readonly IList<IGenerator> _userGenerators;
	    private readonly IGenerator _defaultGenerator;
	    private readonly ReusableInstancesGenerator _instancesGenerator;
		private int _maximumDepth = 20;
	    private bool _sealed;

	    public Builder(int seed)
		{
			Seed = seed;
            _userGenerators = new List<IGenerator>();

            _instancesGenerator = new ReusableInstancesGenerator(seed);
		    _defaultGenerator = new DefaultConstructorGenerator();
			WithGenerator(new ListGenerator(seed, 10, 20));
			WithGenerator(new ArrayGenerator(seed, 10, 20));
			WithGenerator(new NullableTypeGenerator(seed));
			WithGenerator(new DateTimeGenerator(seed));
			WithGenerator(new EnumGenerator(seed));
			WithGenerator(new BoolGenerator(seed));
			WithGenerator(new IntGenerator(seed));
			WithGenerator(new StringGenerator(seed));
			WithGenerator(new UIntGenerator(seed));
		}
		
        protected Builder(Builder parent) : this(parent.Seed)
        {
            Parent = parent;
        }

	    public IBuilder WithGenerator(IGenerator generator)
		{
	        AssertBuilderIsntSealed();
            // each new generator should be inserted at the front of the queue to allow 
            // easy overriding of which generator to use
			_userGenerators.Insert(0, generator);
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

		public IBuilder SetCollectionSize(int minimum, int maximum)
		{
			AssertBuilderIsntSealed();
            var generators = _userGenerators.Select(g => g as ICollectionGenerator).Where(g => g != null).ToArray();
		    foreach (var generator in generators)
		    {
                generator.MinimumListItems = minimum;
                generator.MaximumListItems = maximum;    
		    }
			return this;	
		}

	    public IBuilder SetNumberOfInstances<TType>(int minimum, int maximum)
	    {
	        _instancesGenerator.SetNumberOfInstances<TType>(minimum, maximum);
	        return this;
	    }

	    #region Set values

        public IBuilder AfterPopulationOf<TType>(Action<TType> setter)
        {
            AssertBuilderIsntSealed();
            Setters.Add(new Setter<TType>(setter));
            return this;
        }

        public IBuilder AfterPopulationOf<TType>(Func<TType, TType> setter)
        {
            AssertBuilderIsntSealed();
            Setters.Add(new Setter<TType>(setter));
            return this;
        }

	    public IBuilder AfterPopulationOf<TType>(Action<TType, IBuilder, BuildSession> setter)
	    {
            AssertBuilderIsntSealed();
            Setters.Add(new Setter<TType>(setter));
            return this;
	    }

	    public IBuilder AfterPopulationOf<TType>(Func<TType, IBuilder, BuildSession, TType> setter)
	    {
            AssertBuilderIsntSealed();
            Setters.Add(new Setter<TType>(setter));
            return this;
	    }

        /// <summary>
        /// http://handcraftsman.wordpress.com/2008/11/11/how-to-get-c-property-names-without-magic-strings/
        /// </summary>
        public IBuilder AfterConstructionOf<TType>(Expression<Func<TType, object>> expression, Func<object, IBuilder, BuildSession, object> value)
        {
            var member = ((MemberExpression)expression.Body).Member;
            switch(member.MemberType)
            {
                case MemberTypes.Property:
                    MemberSetters.Add(new PropertyMemberSetter(member.DeclaringType.GetProperty(member.Name), value));
                    break;
                case MemberTypes.Field:
                    MemberSetters.Add(new FieldMemberSetter(member.DeclaringType.GetField(member.Name), value));
                    break;
                case MemberTypes.Constructor:
                case MemberTypes.Event:
                case MemberTypes.Method:
                case MemberTypes.TypeInfo:
                case MemberTypes.Custom:
                case MemberTypes.NestedType:
                case MemberTypes.All:
                    throw new InvalidDataException("MemberType {0} not supported".With(member.MemberType));
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return this;
        }

	    #endregion
		
	    public IBuilder Sealed()
	    {
	        _sealed = true;
	        return this;
	    }		
		
        public IBuilder CreateNew()
	    {
	        return new Builder(this);
	    }
		
	    public TType Build<TType>()
		{
			return Build<TType>(new BuildSession());
		}

        public TType Build<TType>(BuildSession session)
        {
            return (TType)Build(typeof (TType), session);
        }

        public object Build(Type type)
        {
            return Build(type, new BuildSession());
        }

	    public object Build(Type type, BuildSession session)
        {
            Sealed();
            if (session.BuildDepth > _maximumDepth)
                return null;

	        session.BuildDepth++;
	        var generator = GetGeneratorOrDefault(type, session);
            var obj = generator.Create(type, this, session);
	        session.ConstructedObjects.Add(obj);
            obj = DoMemberSetters(type, obj, session);
            obj = generator.Populate(type, obj, this, session);
            obj = DoGeneralSetters(type, obj, session);
	        session.BuildDepth--;
            return obj;
        }

        private object DoMemberSetters(Type type, object obj, BuildSession session)
        {
            foreach (var property in type.GetProperties())
            {
                var setters = MemberSetters.Where(s => s.IsForMember(property)).ToArray();
                foreach (var setter in setters)
                {
                    session.ConstructedMembers.Add(property);
                    setter.Set(obj, this, session);
                }
            }
            foreach (var field in type.GetFields())
            {
                var setters = MemberSetters.Where(s => s.IsForMember(field)).ToArray();
                foreach (var setter in setters)
                {
                    session.ConstructedMembers.Add(field);
                    setter.Set(obj, this, session);
                }
            }
            return obj;
        }

        private object DoGeneralSetters(Type type, object obj, BuildSession session)
        {
			var setters = Setters.Where(s => s.IsForType(type)).ToArray();
            return setters.Aggregate(obj, (current, setter) => setter.Set(current, this, session));
        }

		private IGenerator GetGeneratorOrDefault(Type type, BuildSession session)
        {
            return GetGenerator(type, session) ?? _defaultGenerator;
        }
		
        private IGenerator GetGenerator(Type type, BuildSession session)
        {
            var thisGenerator = _instancesGenerator.GeneratesType(type, this, session) ? _instancesGenerator : _userGenerators.FirstOrDefault(g => g.GeneratesType(type, this, session));
            if(thisGenerator == null && Parent != null)
            {
                thisGenerator = Parent.GetGenerator(type, session);
            }
            return thisGenerator;
        }
	}

    public class Builder<TType> : Builder, IBuilder<TType>
    {
        public Builder(int seed) : base(seed)
        {}

        protected Builder(Builder builder) : base(builder)
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

        public new IBuilder<TType> AfterConstructionOf<T>(Expression<Func<T, object>> expression, Func<object, IBuilder, BuildSession, object> value)
        {
            base.AfterConstructionOf(expression, value);
            return this;
        }

        public new IBuilder<TType> SetCollectionSize(int minimum, int maximum)
		{
			base.SetCollectionSize(minimum, maximum);
			return this;
		}

        public new IBuilder<TType> SetNumberOfInstances<TType1>(int minimum, int maximum)
        {
            base.SetNumberOfInstances<TType1>(minimum, maximum);
            return this;
        }

        public new IBuilder<TType> AfterPopulationOf<TType1>(Action<TType1> setter)
        {
            base.AfterPopulationOf(setter);
            return this;
        }

        public new IBuilder<TType> AfterPopulationOf<TType1>(Func<TType1, TType1> setter)
        {
            base.AfterPopulationOf(setter);
            return this;
        }

        public new IBuilder<TType> AfterPopulationOf<TType1>(Action<TType1, IBuilder, BuildSession> setter)
        {
            base.AfterPopulationOf(setter);
            return this;
        }

        public new IBuilder<TType> AfterPopulationOf<TType1>(Func<TType1, IBuilder, BuildSession, TType1> setter)
        {
            base.AfterPopulationOf(setter);
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

        public TType Build(BuildSession session)
        {
            return Build<TType>(session);
        }

    }
}
