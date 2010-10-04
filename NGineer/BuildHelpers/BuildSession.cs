using System;
using System.Reflection;
using System.Collections.Generic;

namespace NGineer.BuildHelpers
{
    public class BuildSession
    {
		private readonly Random _random;
        private readonly IBuilder _builder;
        private readonly ITypeRegistry<Range> _collectionSizes;
		private readonly ITypeRegistry<int?> _maxInstances;
        private readonly Range _defaultCollectionSize;
        private readonly ObjectBuildTreeEntry _objectTreeRoot;
        private readonly IList<ObjectBuildTreeEntry> _constructedNodes;
		private readonly Stack<MemberInfo> _memberStack;

        protected BuildSession(IBuilder builder, 
		                       ITypeRegistry<Range> collectionSizes, 
		                       ITypeRegistry<int?> maxInstances, 
		                       Range defaultCollectionSize, 
		                       bool ignored)
        {
            _builder = builder;
            _collectionSizes = collectionSizes;
			_maxInstances = maxInstances;
            _defaultCollectionSize = defaultCollectionSize;
        }

        public BuildSession(IBuilder builder, 
		                    ITypeRegistry<Range> collectionSizes, 
		                    ITypeRegistry<int?> maxInstances,
		                    Range defaultCollectionSize,
		                    Random random)
            : this(builder, collectionSizes, maxInstances, defaultCollectionSize, false)
        {
			_random = random;
            _constructedNodes = new List<ObjectBuildTreeEntry>();
            _objectTreeRoot = new ObjectBuildTreeEntry(null, null, -1);
			_memberStack = new Stack<MemberInfo>();
            CurrentObject = _objectTreeRoot;
        }

        public BuildSession(IBuilder builder, 
		                    ITypeRegistry<Range> collectionSizes,
		                    ITypeRegistry<int?> maxInstances,
		                    Range defaultCollectionSize, 
		                    BuildSession parent)
            : this(builder, collectionSizes, maxInstances, defaultCollectionSize, false)
        {
			_random = parent._random;
            _objectTreeRoot = parent._objectTreeRoot;
            _constructedNodes = parent._constructedNodes;
            CurrentObject = parent.CurrentObject;
			_memberStack = parent._memberStack;
        }

        #region Readonly Properties
        public IBuilder Builder { get { return _builder; } }
        public ObjectBuildTreeEntry BuiltObjectTreeRoot { get { return _objectTreeRoot; } }
        public int BuildDepth { get { return CurrentObject.Depth; } }
        public IList<ObjectBuildTreeEntry> ConstructedNodes { get { return _constructedNodes; } }
		public Random Random { get { return _random; } }
        public MemberInfo[] CurrentMemberStack { get { return _memberStack.ToArray(); } }
		
        #endregion

        public ObjectBuildTreeEntry CurrentObject { get; private set; }

		public MemberInfo CurrentMember { get; private set; }

        public int ConstructedCount { get; private set; }

        public void PushObject(Type type, object  obj)
        {
            var buildRecord = obj as ObjectBuildRecord;
            if(buildRecord != null)
            {
                // On the second pass through, this object is already being populated, so does not require further population
                buildRecord.RequiresPopulation = false; 
            }
            else
            {
                if (BuilderInstanceTracker.IncludeInCount(type))
                    ConstructedCount++;
                buildRecord = new ObjectBuildRecord(type, obj);
            }
            CurrentObject = CurrentObject.AddChild(buildRecord);
            _constructedNodes.Add(CurrentObject);
        }

        public void PopObject()
        {
            if (CurrentObject.Parent == null)
                throw new BuilderException("Unable to pop beyond the root element of the built object tree");
            CurrentObject.RequiresPopulation = false;
            CurrentObject = CurrentObject.Parent;
        }
		
		public void PushMember(MemberInfo property)
		{
			_memberStack.Push(property);
			CurrentMember = property;
		}
		
		public void PopMember(bool valueHasChanged)
		{
			var member = _memberStack.Pop();
			CurrentMember = _memberStack.Count > 0 ? _memberStack.Peek() : null;
			if(valueHasChanged)
			{
				CurrentObject.RegisterConstructed(member);
			}
		}

        public Range GetCollectionSize(Type type)
        {
            return _collectionSizes.GetForType(type) ?? _defaultCollectionSize;
        }
		
		public int? GetMaximumNumberOfInstances(Type type)
		{
			return _maxInstances.GetForType(type);
		}
    }
}