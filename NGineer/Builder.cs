using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using NGineer.BuildGenerators;
using NGineer.Internal;
using NGineer.Utils;

namespace NGineer
{
    public class Builder : IConfiguredBuilder
    {
        public static class Defaults
        {
            public const int BuildDepth = 5;
            public static readonly Range CollectionSize = new Range(10, 20);
            public static int MaximumObjects = 5000;
        }

        private readonly Builder _parent;
        private readonly IList<IInstanceProcessor> _setters = new List<IInstanceProcessor>();
        private readonly IList<IMemberSetter> _memberSetters = new List<IMemberSetter>();
        private readonly IList<Action<BuildSession>> _postbuildHooks;
        private readonly int _seed;
        private readonly Random _random;
        private readonly TypeRegistry<Range> _collectionSizes;
        private readonly TypeRegistry<int?> _maxInstances;
        private readonly IDictionary<Type, bool> _ignoreUnset;
        private readonly IList<IGenerator> _generators;
        private readonly DefaultReusableInstancesGenerator _instancesGenerator;
        private readonly BuildSession _session;

        // Inherited properties
        private readonly ITypeRegistry<Range> _allCollectionSizes;
        private readonly ITypeRegistry<int?> _allMaxInstances;

        private Range _defaultCollectionSize;
        private int? _maximumDepth;
        private int? _maximumObjects;
        private bool _sealed;
        private bool? _throwOnDepthLimitReached;


        protected Builder(int seed, Builder parent, BuildSession session)
        {
            _seed = seed;
            _session = session;
            _random = new Random(seed);
            _maxInstances = new TypeRegistry<int?>();
            _collectionSizes = new TypeRegistry<Range>();
            _ignoreUnset = new Dictionary<Type, bool>();
            _generators = new List<IGenerator>();
            _instancesGenerator = new DefaultReusableInstancesGenerator();
            _postbuildHooks = new List<Action<BuildSession>>();
            _parent = parent;
            if(_parent != null)
            {
                _allMaxInstances = new InheritedTypeRegistry<int?>(_parent._allMaxInstances, _maxInstances);
                _allCollectionSizes = new InheritedTypeRegistry<Range>(_parent._allCollectionSizes, _collectionSizes);
            }

            else
            {
                _allMaxInstances = new InheritedTypeRegistry<int?>(null, _maxInstances);
                _allCollectionSizes = new InheritedTypeRegistry<Range>(null, _collectionSizes);
            }
        }

        public Builder(int seed) :  this(seed, null, null)
        {
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

        public Builder() : this(0)
        {
        }

        #region Properties that take into account parent settings as well

        public IEnumerable<Action<BuildSession>> PostBuildHooks
        {
            get
            {
                if(_parent != null)
                    return _parent.PostBuildHooks.Union(_postbuildHooks);
                return _postbuildHooks;
            }
        }

        public IEnumerable<IInstanceProcessor> Setters
        {
            get
            {
                if(_parent != null)
                    return _parent.Setters.Union(_setters);
                return _setters;
            }
        }

        public IEnumerable<IMemberSetter> MemberSetters
        {
            get
            {
                if(_parent != null)
                    return _memberSetters.Union(_parent.MemberSetters);
                return _memberSetters;
            }
        }

        public int BuildDepth
        {
            get
            {
                if(_maximumDepth.HasValue)
                {
                    return _maximumDepth.Value;
                }
                if(_parent != null)
                {
                    return _parent.BuildDepth;
                }
                return Defaults.BuildDepth;
            }
        }

        public bool ThrowWhenBuildDepthReached
        {
            get
            {
                if(_throwOnDepthLimitReached.HasValue)
                {
                    return _throwOnDepthLimitReached.Value;
                }
                if(_parent != null)
                {
                    return _parent.ThrowWhenBuildDepthReached;
                }
                return IsBuildDepthUnset;
            }
        }

        public bool IsBuildDepthUnset
        {
            get { return _maximumDepth == null && (_parent == null || _parent.IsBuildDepthUnset); }
        }


        public int MaximumObjects
        {
            get
            {
                if(_maximumObjects.HasValue)
                {
                    return _maximumObjects.Value;
                }
                if(_parent != null)
                {
                    return _parent.MaximumObjects;
                }
                return Defaults.MaximumObjects;
            }
        }

        public Range DefaultCollectionSize
        {
            get
            {
                if(_defaultCollectionSize != null)
                {
                    return _defaultCollectionSize;
                }
                if(_parent != null)
                {
                    return _parent.DefaultCollectionSize;
                }
                return Defaults.CollectionSize;
            }
        }

        public ITypeRegistry<Range> CollectionSizes
        {
            get { return _allCollectionSizes; }
        }

        public ITypeRegistry<int?> MaxInstances
        {
            get { return _allMaxInstances; }
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
            if(generator == null)
                throw new ArgumentNullException("generator");
            return WithGenerator(new BuilderGenerator(type, generator));
        }

        public IBuilder WithGenerator<TType>(Func<IBuilder, BuildSession, TType> generator)
        {
            if(generator == null)
                throw new ArgumentNullException("generator");
            return WithGenerator(new BuilderGenerator<TType>(generator));
        }

        public IBuilder WithGenerator<TType>(Func<TType> generator)
        {
            if(generator == null)
                throw new ArgumentNullException("generator");
            return WithGenerator(new BuilderGenerator<TType>((b, s) => generator()));
        }
        #endregion

        private void AssertBuilderIsntSealed()
        {
            if(_sealed)
            {
                throw new BuilderSealedException();
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

        #region SetCollectionSize
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
        #endregion

        #region SetNumberOfInstances

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

        #endregion

        #region AfterPopulationOf implementations

        public IBuilder AfterPopulationOf(IInstanceProcessor setter)
        {
            if(setter == null)
                throw new ArgumentNullException("setter");
            AssertBuilderIsntSealed();
            _setters.Add(setter);
            return this;
        }

        #endregion

        #region AfterConstructionOf implementations

        public IBuilder AfterConstructionOf(IMemberSetter setter)
        {
            AssertBuilderIsntSealed();
            if(setter == null)
                throw new ArgumentNullException("setter");
            _memberSetters.Insert(0, setter);
            return this;
        }

        #endregion

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

        public IBuilder PostBuild(Action<BuildSession> hook)
        {
            AssertBuilderIsntSealed();
            _postbuildHooks.Add(hook);
            return this;
        }

        public IBuilder Sealed()
        {
            _sealed = true;
            if(_parent != null)
            {
                _parent.Sealed();
            }
            return this;
        }

        public IBuilder CreateNew()
        {
            return CreateNew(null);
        }

        public IConfiguredBuilder CreateNew(BuildSession session)
        {
            return new Builder(this._seed + 1, this, session);
        }

        #region Build implementations
        public object Build(Type type)
        {
            Sealed();
            using (var session = new BuildSession(this, _session, _random))
            {
                var obj = session.Build(type);
                foreach (var hook in PostBuildHooks)
                {
                    hook(session);
                }
                return obj;
            }
        }

        public TType Build<TType>()
        {
            return (TType)Build(typeof(TType));
        }
        #endregion

        #region Query methods used during construction
        public bool ShouldIgnoreUnset(Type type)
        {
            bool result = false;
            if(!_ignoreUnset.TryGetValue(type, out result) && _parent != null)
            {
                result = _parent.ShouldIgnoreUnset(type);
            }
            return result;
        }

        public IGenerator GetGenerator(Type type, BuildSession session)
        {
            var thisGenerator = _instancesGenerator.GeneratesType(type, session.Builder, session) ? _instancesGenerator : _generators.LastOrDefault(g => g.GeneratesType(type, session.Builder, session));
            if(thisGenerator == null && _parent != null)
            {
                thisGenerator = _parent.GetGenerator(type, session);
            }
            return thisGenerator;
        }
        #endregion
    }
}
