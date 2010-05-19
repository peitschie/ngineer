using System;
using System.Collections.Generic;

namespace NGineer.BuildHelpers
{
    public class BuildSession
    {
        private readonly IBuilder _builder;
        private readonly ITypeRegistry<Range> _collectionSizes;
        private readonly Range _defaultCollectionSize;
        private readonly ObjectBuildTreeEntry _objectTreeRoot;
        private readonly IList<ObjectBuildTreeEntry> _constructedNodes;

        protected BuildSession(IBuilder builder, ITypeRegistry<Range> collectionSizes, Range defaultCollectionSize, bool ignored)
        {
            _builder = builder;
            _collectionSizes = collectionSizes;
            _defaultCollectionSize = defaultCollectionSize;
        }

        public BuildSession(IBuilder builder, ITypeRegistry<Range> collectionSizes, Range defaultCollectionSize)
            : this(builder, collectionSizes, defaultCollectionSize, false)
        {
            _constructedNodes = new List<ObjectBuildTreeEntry>();
            _objectTreeRoot = new ObjectBuildTreeEntry(null, null, -1);
            CurrentObject = _objectTreeRoot;
        }

        public BuildSession(IBuilder builder, ITypeRegistry<Range> collectionSizes, Range defaultCollectionSize, BuildSession parent)
            : this(builder, collectionSizes, defaultCollectionSize, false)
        {
            _objectTreeRoot = parent._objectTreeRoot;
            _constructedNodes = parent._constructedNodes;
            CurrentObject = parent.CurrentObject;
        }

        #region Readonly Properties
        public IBuilder Builder { get { return _builder; } }
        public ObjectBuildTreeEntry BuiltObjectTreeRoot { get { return _objectTreeRoot; } }
        public int BuildDepth { get { return CurrentObject.Depth; } }
        public IList<ObjectBuildTreeEntry> ConstructedNodes { get { return _constructedNodes; } }

        #endregion

        public ObjectBuildTreeEntry CurrentObject { get; private set; }

        public void PushChild(ObjectBuildRecord obj)
        {
            CurrentObject = CurrentObject.AddChild(obj);
            _constructedNodes.Add(CurrentObject);
        }

        public void PushChild(Type type, object  obj)
        {
            PushChild((obj as ObjectBuildRecord) ?? new ObjectBuildRecord(type, obj));
        }

        public void PopChild()
        {
            if (CurrentObject.Parent == null)
                throw new BuilderException("Unable to pop beyond the root element of the built object tree");
            CurrentObject.IsPopulated = true;
            CurrentObject = CurrentObject.Parent;
        }

        public Range GetCollectionSize(Type type)
        {
            return _collectionSizes.GetForType(type) ?? _defaultCollectionSize;
        }
    }
}