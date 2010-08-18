using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using NGineer.BuildGenerators;
using NGineer.BuildHelpers;
using NGineer.Utils;

namespace NGineer
{
	public class Builder : IBuilder
	{
        public static class Defaults
        {
            public const int BuildDepth = 5;
            public static readonly Range CollectionSize = new Range(10, 20);
            public static int MaximumObjects = 5000;
        }
		
        protected readonly Builder Parent;
        protected readonly IList<ISetter> Setters = new List<ISetter>();
        protected readonly IList<IMemberSetter> MemberSetters = new List<IMemberSetter>();

	    protected readonly int Seed;
		private readonly Random _random;
	    private readonly TypeRegistry<Range> _collectionSizes;
		private readonly TypeRegistry<int?> _maxInstances;
        private readonly IDictionary<Type, bool> _ignoreUnset;
	    private readonly IList<IGenerator> _userGenerators;
	    private readonly IGenerator _defaultGenerator;
	    private readonly DefaultReusableInstancesGenerator _instancesGenerator;
		
		private Range _defaultCollectionSize;
		private int? _maximumDepth;
        private int? _maximumObjects;
	    private bool _sealed;
	    private bool? _throwOnDepthLimitReached;
		
		// Inherited properties
		private readonly ITypeRegistry<Range> _allCollectionSizes;
		private readonly ITypeRegistry<int?> _allMaxInstances;

	    protected Builder(int seed, bool ignored)
        {
            Seed = seed;
			_random = new Random(seed);
			_maxInstances = new TypeRegistry<int?>();
            _collectionSizes = new TypeRegistry<Range>();
            _ignoreUnset = new Dictionary<Type, bool>();
            _userGenerators = new List<IGenerator>();
            _instancesGenerator = new DefaultReusableInstancesGenerator();
            _defaultGenerator = new DefaultConstructorGenerator();
        }

        public Builder() : this(0)
        {}

	    public Builder(int seed) : this(seed, false)
		{
			_allMaxInstances = new InheritedTypeRegistry<int?>(null, _maxInstances);
            _allCollectionSizes = new InheritedTypeRegistry<Range>(null, _collectionSizes);
			WithGenerator(new ListGenerator());
            WithGenerator(new ArrayGenerator());
            WithGenerator(new NullableTypeGenerator());
            WithGenerator(new DateTimeGenerator());
            WithGenerator(new EnumGenerator());
            WithGenerator(new BoolGenerator());
            WithGenerator(new CharGenerator());
            WithGenerator(new BlittableTypesGenerator());
            WithGenerator(new SimpleMemberNameStringGenerator());
		}

        protected Builder(Builder parent)
            : this(parent.Seed + 1, false)
        {
            Parent = parent;
			_allMaxInstances = new InheritedTypeRegistry<int?>(Parent._maxInstances, 
			                                                   _maxInstances);
            _allCollectionSizes = new InheritedTypeRegistry<Range>(Parent._collectionSizes,
                                                                   _collectionSizes);
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


        public int MaximumObjects
        {
            get
            {
                if (_maximumObjects.HasValue)
                {
                    return _maximumObjects.Value;
                }
                if (Parent != null)
                {
                    return Parent.MaximumObjects;
                }
                return Defaults.MaximumObjects;
            }
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
		
		protected ITypeRegistry<int?> MaxInstances
		{
			get
			{
				return _allMaxInstances;
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

	    public IBuilder SetMaximumObjects(int? objects)
	    {
            AssertBuilderIsntSealed();
	        _maximumObjects = objects;
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
            _collectionSizes.SetForType(type, new Range(minimum, maximum));
            return this;
        }

		public IBuilder SetCollectionSize<TType>(int minimum, int maximum)
		{
			return SetCollectionSize(typeof(TType), minimum, maximum);
        }
		
        public IBuilder SetNumberOfInstances(Type type, int minimum, int maximum)
        {
            _maxInstances.SetForType(type, _random.NextInRange(minimum, maximum));
            return this;
        }

	    #region AfterPopulationOf implementations

	    public IBuilder AfterPopulationOf(ISetter setter)
	    {
			if(setter == null)
				throw new ArgumentNullException("setter");
	        AssertBuilderIsntSealed();
            Setters.Add(setter);
            return this;
	    }

        #endregion

        #region AfterConstructionOf implementations

        public IBuilder AfterConstructionOf(IMemberSetter setter)
		{
			if(setter == null)
				throw new ArgumentNullException("setter");
			MemberSetters.Insert(0, setter);
			return this;
		}

	    #endregion


        public IBuilder IgnoreMember(MemberInfo member)
        {
            AfterConstructionOf(new IgnoreMemberSetter(member));
            return this;
        }

	    public IBuilder IgnoreUnset(Type type)
	    {
	        _ignoreUnset[type] = true;
            return this;
	    }

	    public ITypedBuilder<TType> For<TType>()
        {
            return new TypedBuilder<TType>(this);
        }

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
        public object Build(Type type)
        {
            return Build(type, new BuildSession(this, CollectionSizes, 
			                                    MaxInstances,
			                                    DefaultCollectionSize,
			                                    _random));
        }

	    public object Build(Type type, BuildSession session)
        {
            Sealed();
	        var internalSession = ReferenceEquals(this, session.Builder)
	                                  ? session
	                                  : new BuildSession(this, CollectionSizes, MaxInstances, 
					                   							DefaultCollectionSize, session);

            if (internalSession.BuildDepth == BuildDepth)
            {
                if (IsBuildDepthUnset || ThrowWhenBuildDepthReached)
                {
                    throw new BuilderDepthExceededException(BuildDepth, internalSession);
                }
                return null;
            }
            if (internalSession.ConstructedCount > MaximumObjects)
            {
                throw new BuilderMaximumInstancesReached(MaximumObjects, internalSession);
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
                    if (!_ignoreUnset.ContainsKey(type) || _ignoreUnset[type] == false)
                    {
                        generator.Populate(type, obj, this, internalSession);
                        DoPopulators(type, internalSession);
                    }
                }
                internalSession.PopObject();
            }
	        return obj;
        }

		public TType Build<TType>()
		{
			return (TType)Build(typeof(TType));
		}
		
		public TType Build<TType>(BuildSession session)
		{
			return (TType)Build(typeof(TType), session);
		}		
	    #endregion

        private void DoMemberSetters(Type type, BuildSession session)
        {
            foreach (var member in session.CurrentObject.UnconstructedMembers)
            {
				session.PushMember(member);
                var setter = MemberSetters.FirstOrDefault(s => s.IsForMember(member, session.Builder, session));
                if(setter != null)
                    setter.Set(session.CurrentObject.Object, session.Builder, session);
				session.PopMember(setter != null);
            }
            if (Parent != null)
            {
                Parent.DoMemberSetters(type, session);
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
