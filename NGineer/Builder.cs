using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using NGineer.BuildGenerators;
using NGineer.BuildHelpers;
using NGineer.SpecialGenerators;
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
	    private readonly TypeRegistry<Range> _builderCollectionSizes;
	    private readonly ITypeRegistry<Range> _allCollectionSizes;
	    private readonly IList<IGenerator> _userGenerators;
	    private readonly IGenerator _defaultGenerator;
	    private readonly ReusableInstancesGenerator _instancesGenerator;
		private int? _maximumDepth;
	    private bool _sealed;
	    private bool? _throwOnDepthLimitReached;

	    protected Builder(int seed, bool ignored)
        {
            Seed = seed;
            _builderCollectionSizes = new TypeRegistry<Range>();
            _userGenerators = new List<IGenerator>();
            _instancesGenerator = new ReusableInstancesGenerator(seed);
            _defaultGenerator = new DefaultConstructorGenerator();
        }

	    public Builder(int seed) : this(seed, false)
		{
            _allCollectionSizes = new InheritedTypeRegistry<Range>(null, _builderCollectionSizes);
			WithGenerator(new ListGenerator(seed));
            WithGenerator(new ArrayGenerator(seed));
            WithGenerator(new NullableTypeGenerator(seed));
            WithGenerator(new DateTimeGenerator(seed));
            WithGenerator(new EnumGenerator(seed));
            WithGenerator(new BoolGenerator(seed));
            WithGenerator(new CharGenerator(seed));
            WithGenerator(new BlittableTypesGenerator(seed));
            WithGenerator(new SimpleMemberNameStringGenerator());
            WithGenerator(new UIntGenerator(seed));
		}

        protected Builder(Builder parent)
            : this(parent.Seed + 1, false)
        {
            Parent = parent;
            _allCollectionSizes = new InheritedTypeRegistry<Range>(Parent._builderCollectionSizes,
                                                                   _builderCollectionSizes);
        }

	    #region Properties that take into account parent settings as well
        public int BuildDepth
		{
			get 
			{
                if (_maximumDepth.HasValue)
                {
                    return _maximumDepth.Value;
                }
                if (Parent != null)
                {
                    return Parent.BuildDepth;
                }
                return Defaults.BuildDepth;
			}
		}

	    protected bool ThrowWhenBuildDepthReached
	    {
	        get
	        {
                if(_throwOnDepthLimitReached.HasValue)
                {
                    return _throwOnDepthLimitReached.Value;
                }
                if(Parent != null)
                {
                    return Parent.ThrowWhenBuildDepthReached;
                }
	            return IsBuildDepthUnset;
	        }
	    }

	    protected bool IsBuildDepthUnset
	    {
            get { return _maximumDepth == null && (Parent == null || Parent.IsBuildDepthUnset); }
	    }

	    public Range DefaultCollectionSize
	    {
            get
            {
                if (_defaultCollectionSize != null)
                {
                    return _defaultCollectionSize;
                }
                if (Parent != null)
                {
                    return Parent.DefaultCollectionSize;
                }
                return Defaults.CollectionSize;
            }
	    }

	    protected ITypeRegistry<Range> CollectionSizes
	    {
	        get
	        {
	            return _allCollectionSizes;
	        }
	    }
        #endregion


        #region WithGenerator implementations
        public IBuilder WithGenerator(IGenerator generator)
		{
	        AssertBuilderIsntSealed();
            // each new generator should be inserted at the front of the queue to allow 
            // easy overriding of which generator to use
			_userGenerators.Insert(0, generator);
			return this;
		}

        public IBuilder WithGenerator(Type type, Func<IBuilder, BuildSession, object> generator)
        {
            return WithGenerator(new BuilderGenerator(type, generator));
        }

        public IBuilder WithGenerator<TType>(Func<IBuilder, BuildSession, TType> generator)
        {
            return WithGenerator(new BuilderGenerator<TType>(generator));
        }        
        
        public IBuilder WithGenerator<TType>(Func<TType> generator)
        {
            return WithGenerator(new BuilderGenerator<TType>((b, s) => generator()));
        }
        #endregion

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

        public IBuilder ThrowsWhenMaximumDepthReached()
        {
            _throwOnDepthLimitReached = true;
            return this;
        }

		public IBuilder SetCollectionSize(int minimum, int maximum)
		{
			AssertBuilderIsntSealed();
		    _defaultCollectionSize = new Range(minimum, maximum);
			return this;	
		}

        public IBuilder SetCollectionSize(Type type, int minimum, int maximum)
        {
            _builderCollectionSizes.SetForType(type, new Range(minimum, maximum));
            return this;
        }

	    public IBuilder SetCollectionSize<TType>(int minimum, int maximum)
	    {
	        _builderCollectionSizes.SetForType<TType>(new Range(minimum, maximum));
	        return this;
	    }

        public IBuilder SetNumberOfInstances(Type type, int minimum, int maximum)
        {
             _instancesGenerator.SetNumberOfInstances(type, minimum, maximum);
            return this;
        }

	    public IBuilder SetNumberOfInstances<TType>(int minimum, int maximum)
	    {
	        _instancesGenerator.SetNumberOfInstances<TType>(minimum, maximum);
	        return this;
	    }

	    #region AfterPopulationOf implementations

	    public IBuilder AfterPopulationOf<TType>(Action<TType> setter)
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

	    public IBuilder AfterPopulationOf(ISetter setter)
	    {
	        AssertBuilderIsntSealed();
            Setters.Add(setter);
            return this;
	    }

        #endregion

        #region AfterConstructionOf implementations

        public IBuilder AfterConstructionOf(IMemberSetter setter)
		{
			MemberSetters.Add(setter);
			return this;
		}

        public IBuilder AfterConstructionOf(MemberInfo member, Func<object, IBuilder, BuildSession, object> value)
        {
            ValidateMember(member, value);
            switch (member.MemberType)
            {
                case MemberTypes.Property:
                    MemberSetters.Add(new PropertyMemberSetter((PropertyInfo) member, value));
                    break;
                case MemberTypes.Field:
                    MemberSetters.Add(new FieldMemberSetter((FieldInfo)member, value));
                    break;
            }
            return this;
        }

        public IBuilder AfterConstructionOf<TType, TReturnType>(Expression<Func<TType, TReturnType>> expression, Func<TType, IBuilder, BuildSession, TReturnType> value)
        {
            // http://handcraftsman.wordpress.com/2008/11/11/how-to-get-c-property-names-without-magic-strings/
            var member = MemberExpressions.GetMemberInfo(expression);
            ValidateMember(member, value);
            switch (member.MemberType)
            {
                case MemberTypes.Property:
                    MemberSetters.Add(new PropertyMemberSetter<TType, TReturnType>((PropertyInfo)member, value));
                    break;
                case MemberTypes.Field:
                    MemberSetters.Add(new FieldMemberSetter<TType, TReturnType>((FieldInfo)member, value));
                    break;
            }
            return this;
        }
       
        public IBuilder AfterConstructionOf<TType, TReturnType>(Expression<Func<TType, TReturnType>> expression, TReturnType value)
        {
            return AfterConstructionOf(expression, (o, b, s) => value);
        }

        private static void ValidateMember<TType, TReturnType>(MemberInfo member, Func<TType, IBuilder, BuildSession, TReturnType> setter)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Property:
                    var propInfo = (PropertyInfo)member;
                    if (!propInfo.PropertyType.IsAssignableFrom(setter.Method.ReturnType))
                    {
                        throw new InvalidCastException("Unable to cast from {0} to {1}".With(setter.Method.ReturnType, propInfo.PropertyType));
                    }
                    break;
                case MemberTypes.Field:
                    var fieldInfo = (FieldInfo)member;
                    if (!fieldInfo.FieldType.IsAssignableFrom(setter.Method.ReturnType))
                    {
                        throw new InvalidCastException("Unable to cast from {0} to {1}".With(setter.Method.ReturnType, fieldInfo.FieldType));
                    }
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

        #region Build implementations
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
            return Build(type, new BuildSession(this, CollectionSizes, DefaultCollectionSize));
        }

	    public object Build(Type type, BuildSession session)
        {
            Sealed();
	        var internalSession = ReferenceEquals(this, session.Builder)
	                                  ? session
	                                  : new BuildSession(this, CollectionSizes, DefaultCollectionSize, session);

            if (internalSession.BuildDepth == BuildDepth)
            {
                if (IsBuildDepthUnset || ThrowWhenBuildDepthReached)
                {
                    throw new BuilderDepthExceededException(BuildDepth, internalSession);
                }
                return null;
            }

	        var generator = GetGeneratorOrDefault(type, internalSession);
            var obj = generator.Create(type, this, internalSession);
            if (obj != null)
            {
                internalSession.PushObject(type, obj);
                obj = internalSession.CurrentObject.Object;
                if (!internalSession.CurrentObject.IsPopulated)
                {
                    DoMemberSetters(type, internalSession);
                    generator.Populate(type, obj, this, internalSession);
                    DoPopulators(type, internalSession);
                }
                internalSession.PopObject();
            }
	        return obj;
        }

        #endregion

        private void DoMemberSetters(Type type, BuildSession session)
        {
            foreach (var property in session.CurrentObject.UnconstructedProperties)
            {
				session.PushMember(property);
                var setters = MemberSetters.Where(s => s.IsForMember(property)).ToArray();
                foreach (var setter in setters)
                {
                    setter.Set(session.CurrentObject.Object, session.Builder, session);
                }
				session.PopMember(setters.Length > 0);
            }
            if (Parent != null)
            {
                Parent.DoMemberSetters(type, session);
            }
            foreach (var field in session.CurrentObject.UnconstructedFields)
            {
				session.PushMember(field);
                var setters = MemberSetters.Where(s => s.IsForMember(field)).ToArray();
                foreach (var setter in setters)
                {
                    setter.Set(session.CurrentObject.Object, session.Builder, session);
                }
                session.PopMember(setters.Length > 0);
            }
        }

        private void DoPopulators(Type type, BuildSession session)
        {
            if(Parent != null)
            {
                Parent.DoPopulators(type, session);
            }
			foreach(var setter in Setters.Where(s => s.IsForType(type)).ToArray())
			{
			    setter.Set(session.CurrentObject.Object, session.Builder, session);
			}
        }

		private IGenerator GetGeneratorOrDefault(Type type, BuildSession session)
        {
            return GetGenerator(type, session) ?? _defaultGenerator;
        }
		
        private IGenerator GetGenerator(Type type, BuildSession session)
        {
            var thisGenerator = _instancesGenerator.GeneratesType(type, session.Builder, session) ? 
				_instancesGenerator : _userGenerators.FirstOrDefault(g => g.GeneratesType(type, session.Builder, session));
            if(thisGenerator == null && Parent != null)
            {
                thisGenerator = Parent.GetGenerator(type, session);
            }
            return thisGenerator;
        }
	}
}
