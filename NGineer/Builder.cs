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
        protected static class Defaults
        {
            public const int BuildDepth = 5;
            public static readonly Range CollectionSize = new Range(10, 20);
        }
		
        protected readonly Builder Parent;
        protected readonly IList<ISetter> Setters = new List<ISetter>();
        protected readonly IList<IMemberSetter> MemberSetters = new List<IMemberSetter>();

	    protected readonly int Seed;
	    private Range _defaultCollectionSize;
	    private readonly TypeRegistry<Range> _collectionSizes;
	    private readonly IList<IGenerator> _userGenerators;
	    private readonly IGenerator _defaultGenerator;
	    private readonly ReusableInstancesGenerator _instancesGenerator;
		private int? _maximumDepth;
	    private bool _sealed;

	    public Builder(int seed)
		{
			Seed = seed;
            _collectionSizes = new TypeRegistry<Range>();
            _userGenerators = new List<IGenerator>();
            _instancesGenerator = new ReusableInstancesGenerator(seed);
		    _defaultGenerator = new DefaultConstructorGenerator();
			WithGenerator(new ListGenerator(seed));
			WithGenerator(new ArrayGenerator(seed));
			WithGenerator(new NullableTypeGenerator(seed));
			WithGenerator(new DateTimeGenerator(seed));
			WithGenerator(new EnumGenerator(seed));
			WithGenerator(new BoolGenerator(seed));
			WithGenerator(new CharGenerator(seed));
			WithGenerator(new BlittableTypesGenerator(seed));
			WithGenerator(new StringGenerator(seed));
			WithGenerator(new UIntGenerator(seed));
		}

        protected Builder(Builder parent)
            : this(parent.Seed + 1)
        {
            Parent = parent;
            _collectionSizes = new TypeRegistry<Range>(Parent._collectionSizes);
        }

        #region Properties that take into account parent settings as well
        public int BuildDepth
		{
			get 
			{ 
				int? depth = _maximumDepth;
				if(_maximumDepth == null)
				{
					depth = (Parent != null) ? Parent.BuildDepth : Defaults.BuildDepth;
				}
				return depth.Value;
			}
		}

	    public Range DefaultCollectionSize
	    {
            get
            {
                var range = _defaultCollectionSize;
                if(range == null)
                {
                    range = (Parent != null) ? Parent.DefaultCollectionSize : Defaults.CollectionSize;
                }
                return range;
            }
	    }
        #endregion

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

	    public IBuilder SetMaximumDepth(int? depth)
		{
            AssertBuilderIsntSealed();
			_maximumDepth = depth;
			return this;
		}

		public IBuilder SetCollectionSize(int minimum, int maximum)
		{
			AssertBuilderIsntSealed();
		    _defaultCollectionSize = new Range(minimum, maximum);
			return this;	
		}

	    public IBuilder SetCollectionSize<TType>(int minimum, int maximum)
	    {
	        _collectionSizes.SetForType<TType>(new Range(minimum, maximum));
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

        public IBuilder AfterPopulationOf(ISetter setter)
	    {
	        AssertBuilderIsntSealed();
            Setters.Add(setter);
            return this;
	    }

		public IBuilder AfterConstructionOf(IMemberSetter setter)
		{
			MemberSetters.Add(setter);
			return this;
		}
		
	    public IBuilder AfterConstructionOf<TType, TFuncType>(Expression<Func<TType, object>> expression, Func<TFuncType, IBuilder, BuildSession, object> value)
        {
            // http://handcraftsman.wordpress.com/2008/11/11/how-to-get-c-property-names-without-magic-strings/
            var member = MemberExpressions.GetMemberInfo(expression);
            switch (member.MemberType)
            {
                case MemberTypes.Property:
                    MemberSetters.Add(new PropertyMemberSetter<TFuncType>(member.DeclaringType.GetProperty(member.Name), value));
                    break;
                case MemberTypes.Field:
                    MemberSetters.Add(new FieldMemberSetter<TFuncType>(member.DeclaringType.GetField(member.Name), value));
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

        public IBuilder AfterConstructionOf<TType>(Expression<Func<TType, object>> expression, Func<object, IBuilder, BuildSession, object> value)
        {
            return AfterConstructionOf<TType, object>(expression, value);
        }
        
        public IBuilder AfterConstructionOf<TType>(Expression<Func<TType, object>> expression, object value)
        {
            return AfterConstructionOf<TType, TType>(expression, (o, b, s) => value);
        }

	    #endregion
		
	    public IBuilder Sealed()
	    {
	        _sealed = true;
            if(Parent != null)
            {
                Parent.Sealed();
            }
	        return this;
	    }		
		
        public IBuilder CreateNew()
	    {
	        return new Builder(this);
	    }
		
	    public TType Build<TType>()
		{
            return (TType)Build(typeof(TType));
		}

	    public TType Build<TType>(BuildSession session)
        {
            return (TType)Build(typeof (TType), session);
        }

        public object Build(Type type)
        {
            return Build(type, new BuildSession(_collectionSizes, DefaultCollectionSize));
        }

	    public object Build(Type type, BuildSession session)
        {
            Sealed();
            if (session.BuildDepth > BuildDepth)
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
            if(Parent != null)
            {
                obj = Parent.DoMemberSetters(type, obj, session);
            }
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
            if(Parent != null)
            {
                obj = Parent.DoGeneralSetters(type, obj, session);
            }
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
}
