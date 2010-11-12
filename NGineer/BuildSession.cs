using System;
using System.Reflection;
using System.Collections.Generic;
using NGineer.Internal;

namespace NGineer
{
    public class BuildSession
    {
        private readonly Random _random;
        private readonly IConfiguredBuilder _builder;
        private readonly ObjectBuildTreeEntry _objectTreeRoot;
        private readonly IList<ObjectBuildTreeEntry> _constructedNodes;
        private readonly Stack<MemberInfo> _memberStack;

        protected BuildSession(IConfiguredBuilder builder, Random random, bool ignored)
        {
            _builder = builder;
            _random = random;
        }

        public BuildSession(IConfiguredBuilder builder, Random random) : this(builder, random, false)
        {
            _constructedNodes = new List<ObjectBuildTreeEntry>();
            _objectTreeRoot = new ObjectBuildTreeEntry(null, null, -1);
            _memberStack = new Stack<MemberInfo>();
            CurrentObject = _objectTreeRoot;
        }

        public BuildSession(IConfiguredBuilder builder, BuildSession parent) : this(builder, parent._random, false)
        {
            _constructedNodes = parent._constructedNodes;
            _objectTreeRoot = parent._objectTreeRoot;
            _memberStack = parent._memberStack;
            CurrentObject = parent.CurrentObject;
        }

        #region Readonly Properties
        public IBuilder Builder
        {
            get { return _builder; }
        }
        public ObjectBuildTreeEntry BuiltObjectTreeRoot
        {
            get { return _objectTreeRoot; }
        }
        public int BuildDepth
        {
            get { return CurrentObject.Depth; }
        }
        public IList<ObjectBuildTreeEntry> ConstructedNodes
        {
            get { return _constructedNodes; }
        }
        public Random Random
        {
            get { return _random; }
        }
        public MemberInfo[] CurrentMemberStack
        {
            get { return _memberStack.ToArray(); }
        }

        #endregion

        public ObjectBuildTreeEntry CurrentObject
        {
            get;
            private set;
        }

        public MemberInfo CurrentMember
        {
            get;
            private set;
        }

        public int ConstructedCount
        {
            get;
            private set;
        }

        public void PushObject(Type type, object obj)
        {
            var buildRecord = obj as ObjectBuildRecord;
            if(buildRecord != null)
            {
                // On the second pass through, this object is already being populated, so does not require further population
                buildRecord.RequiresPopulation = false;
            }

            else
            {
                if(BuilderInstanceTracker.IncludeInCount(type))
                    ConstructedCount++;
                buildRecord = new ObjectBuildRecord(type, obj);
            }
            CurrentObject = CurrentObject.AddChild(buildRecord);
            _constructedNodes.Add(CurrentObject);
        }

        public void PopObject()
        {
            if(CurrentObject.Parent == null)
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
            return _builder.CollectionSizes.GetForType(type) ?? _builder.DefaultCollectionSize;
        }

        public int? GetMaximumNumberOfInstances(Type type)
        {
            return _builder.MaxInstances.GetForType(type);
        }
    }
}
