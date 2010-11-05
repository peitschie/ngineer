using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using NGineer.BuildGenerators;
using NGineer.BuildHelpers;
using NGineer.Utils;
using NGineer.Populators;

namespace NGineer
{
	public class Builder : IBuilder
	{
        public static readonly IList<IPopulator> Populators = new IPopulator[]{
            new DefaultPopulator(),
            new ListPopulator(),
            new ArrayPopulator(),
        };

        public static class Defaults
        {
            public const NGineer.BuildOrder BuildOrder = NGineer.BuildOrder.BreadthFirst;
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
	    private readonly IList<IGenerator> _generators;
	    private readonly DefaultReusableInstancesGenerator _instancesGenerator;
		
		private Range _defaultCollectionSize;
		private int? _maximumDepth;
        private int? _maximumObjects;
	    private bool _sealed;
	    private bool? _throwOnDepthLimitReached;
        private BuildOrder? _buildOrder;
		
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
            _generators = new List<IGenerator>();
            _instancesGenerator = new DefaultReusableInstancesGenerator();
        }

        public Builder() : this(0)
        {}

	    public Builder(int seed) : this(seed, false)
		{
			_allMaxInstances = new InheritedTypeRegistry<int?>(null, _maxInstances);
            _allCollectionSizes = new InheritedTypeRegistry<Range>(null, _collectionSizes);
            WithGenerator(new DefaultConstructorGenerator());
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

        protected BuildOrder DefaultBuildOrder
        {
            get
            {
                if(_buildOrder == null && Parent != null)
                    return Parent.DefaultBuildOrder;
                return _buildOrder.HasValue ? _buildOrder.Value : Defaults.BuildOrder;
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

        protected bool ShouldIgnoreUnset(Type type)
        {
            bool result = false;
            if(!_ignoreUnset.TryGetValue(type, out result) && Parent != null)
            {
                result = Parent.ShouldIgnoreUnset(type);
            }
            return result;
        }
        #endregion

        #region WithGenerator implementations
        public IBuilder WithGenerator(IGenerator generator)
		{
	        AssertBuilderIsntSealed();
			_generators.Add(generator);
			return this;
		}

		public IBuilder WithGenerator(Type type, Func<IBuilder, BuildSession, object> generator)
		{
			if (generator == null)
				throw new ArgumentNullException("generator");
			return WithGenerator(new BuilderGenerator(type, generator));
		}

		public IBuilder WithGenerator<TType>(Func<IBuilder, BuildSession, TType> generator)
		{
			if (generator == null)
				throw new ArgumentNullException("generator");
			return WithGenerator(new BuilderGenerator<TType>(generator));
		}

		public IBuilder WithGenerator<TType>(Func<TType> generator)
		{
			if (generator == null)
				throw new ArgumentNullException("generator");
			return WithGenerator(new BuilderGenerator<TType>((b, s) => generator()));
		}		
        #endregion

        private void AssertBuilderIsntSealed()
	    {
            if (_sealed)
            {
                throw new BuilderSealedException();
            }
	    }

        public IBuilder SetBuildOrder(BuildOrder? order)
        {
            AssertBuilderIsntSealed();
            _buildOrder = order;
            return this;
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
            AssertBuilderIsntSealed();
            _throwOnDepthLimitReached = true;
            return this;
        }

		public IBuilder SetDefaultCollectionSize(int minimum, int maximum)
		{
			AssertBuilderIsntSealed();
		    _defaultCollectionSize = new Range(minimum, maximum);
			return this;	
		}

        public IBuilder SetCollectionSize(Type type, int minimum, int maximum)
        {
            AssertBuilderIsntSealed();
            _collectionSizes.SetForType(type, new Range(minimum, maximum));
            return this;
        }

		public IBuilder SetCollectionSize<TType>(int minimum, int maximum)
		{
			return SetCollectionSize(typeof(TType), minimum, maximum);
		}
		
		public IBuilder SetNumberOfInstances<TType>(int minimum, int maximum)
		{
			return SetNumberOfInstances(typeof(TType), minimum, maximum);
		}
		
        public IBuilder SetNumberOfInstances(Type type, int minimum, int maximum)
        {
            AssertBuilderIsntSealed();
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
            AssertBuilderIsntSealed();
        	if (setter == null)
        		throw new ArgumentNullException("setter");
			MemberSetters.Insert(0, setter);
			return this;
		}

	    #endregion


        public IBuilder IgnoreMember(MemberInfo member, bool allowInherited)
        {
            AfterConstructionOf(new IgnoreMemberSetter(member, member.ReflectedType, allowInherited));
            return this;
        }

	    public IBuilder IgnoreUnset(Type type)
	    {
            AssertBuilderIsntSealed();
	        _ignoreUnset[type] = true;
            return this;
	    }

	    public ITypedBuilder<TType> For<TType>()
	    {
	    	return For<TType>(false);
        }

		public ITypedBuilder<TType> For<TType>(bool allowInherited)
		{
            AssertBuilderIsntSealed();
			return new TypedBuilder<TType>(this, allowInherited);
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
                return type.IsValueType ? Activator.CreateInstance(type) : null;
            }
            if (internalSession.ConstructedCount > MaximumObjects)
            {
                throw new BuilderMaximumInstancesReached(MaximumObjects, internalSession);
            }

            var generator = GetGenerator(type, internalSession);
            var obj = generator.Create(type, this, internalSession);
            if (obj != null)
            {
                internalSession.PushObject(type, obj);
                obj = internalSession.CurrentObject.Object;
                if (internalSession.CurrentObject.RequiresPopulation)
                {
                    DoMemberSetters(type, internalSession);
                    if (!ShouldIgnoreUnset(type))
                    {
                        var populator = Populators.LastOrDefault(p => p.PopulatesType(type, this, session));
                        if(populator != null)
                            populator.Populate(type, obj, this, internalSession);
                    }
                    DoPopulators(type, internalSession);
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

        private IGenerator GetGenerator(Type type, BuildSession session)
        {
            var thisGenerator = _instancesGenerator.GeneratesType(type, session.Builder, session) ? 
				_instancesGenerator : _generators.LastOrDefault(g => g.GeneratesType(type, session.Builder, session));
            if(thisGenerator == null && Parent != null)
            {
                thisGenerator = Parent.GetGenerator(type, session);
            }
            return thisGenerator;
        }
	}
}
