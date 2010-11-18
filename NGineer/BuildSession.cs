using System;
using System.Reflection;
using System.Collections.Generic;
using NGineer.Internal;
using System.Linq;
using NGineer.Exceptions;

namespace NGineer
{
    public sealed class BuildSession : IDisposable
    {
        private readonly Random _random;
        private readonly IConfiguredBuilder _builder;
        private readonly ObjectBuildTreeEntry _objectTreeRoot;
        private readonly IList<ObjectBuildTreeEntry> _constructedNodes;
        private readonly Stack<MemberInfo> _memberStack;
        private readonly SessionedBuilder _wrappedBuilder;

        public BuildSession(IConfiguredBuilder builder, BuildSession parent, Random random)
        {
            _builder = builder;
            _wrappedBuilder = new SessionedBuilder(builder, this);
            if (parent != null && !parent.IsDisposed)
            {
                _random = parent._random;
                _constructedNodes = parent._constructedNodes;
                _objectTreeRoot = parent._objectTreeRoot;
                _memberStack = parent._memberStack;
                CurrentObject = parent.CurrentObject;
            }
            else
            {
                _random = random;
                _constructedNodes = new List<ObjectBuildTreeEntry>();
                _objectTreeRoot = new ObjectBuildTreeEntry(null, null, -1);
                _memberStack = new Stack<MemberInfo>();
                CurrentObject = _objectTreeRoot;
            }
        }

        #region Readonly Properties
        public bool IsDisposed {
            get;
            private set;
        }

        public IBuilder Builder
        {
            get { return _wrappedBuilder; }
        }
        public ObjectBuildTreeEntry BuiltObjectTreeRoot
        {
            get { return _objectTreeRoot; }
        }
        public int BuildDepth
        {
            get { return CurrentObject.Depth; }
        }

        public int AvailableBuildDepth
        {
            get { return _builder.BuildDepth - BuildDepth - 1; }
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

        public bool ShouldIgnoreUnset(Type type)
        {
            return _builder.ShouldIgnoreUnset(type);
        }

        public DisposableAction PushObject(ObjectBuildRecord buildRecord)
        {
            if (IsDisposed)
                throw new ObjectDisposedException("BuildSession");
            if (!buildRecord.Counted)
            {
                if (BuilderInstanceTracker.IncludeInCount(buildRecord.Type))
                    ConstructedCount++;
                buildRecord.Counted = true;
            }
            CurrentObject = CurrentObject.AddChild(buildRecord);
            _constructedNodes.Add(CurrentObject);
            return new DisposableAction(PopObject);
        }

        private void PopObject()
        {
            if(IsDisposed)
                throw new ObjectDisposedException("BuildSession");
            if(CurrentObject.Parent == null)
                throw new BuilderException("Unable to pop beyond the root element of the built object tree");
            CurrentObject = CurrentObject.Parent;
        }

        public DisposableAction PushMember(MemberInfo property)
        {
            if(IsDisposed)
                throw new ObjectDisposedException("BuildSession");
            _memberStack.Push(property);
            CurrentMember = property;
            return new DisposableAction(PopMember);
        }

        private void PopMember()
        {
            if(IsDisposed)
                throw new ObjectDisposedException("BuildSession");
            _memberStack.Pop();
            CurrentMember = _memberStack.Count > 0 ? _memberStack.Peek() : null;
        }

        public Range GetCollectionSize(Type type)
        {
            if(IsDisposed)
                throw new ObjectDisposedException("BuildSession");
            return _builder.CollectionSizes.GetForType(type) ?? _builder.DefaultCollectionSize;
        }

        public int? GetMaximumNumberOfInstances(Type type)
        {
            if(IsDisposed)
                throw new ObjectDisposedException("BuildSession");
            return _builder.MaxInstances.GetForType(type);
        }

        public TType Build<TType>()
        {
            return (TType)Build(typeof(TType));
        }

        public object Build(Type type)
        {
            if (IsDisposed)
                throw new ObjectDisposedException("BuildSession");
            if (BuildDepth == _builder.BuildDepth)
            {
                if (_builder.IsBuildDepthUnset || _builder.ThrowWhenBuildDepthReached)
                {
                    throw new DepthExceededException(_builder.BuildDepth, this);
                }
                return type.IsValueType ? Activator.CreateInstance(type) : null;
            }
            if (ConstructedCount > _builder.MaximumObjects)
            {
                throw new MaximumInstancesReachedException(_builder.MaximumObjects, this);
            }

            var generator = _builder.GetGenerator(type, this);
            var obj = generator.CreateRecord(type, this.Builder, this);
            using(PushObject(obj))
            {
                DoMemberSetters();
                DoProcessors(type);
            }
            return obj.Object;
        }

        private void DoMemberSetters()
        {
            var unconstructed = CurrentObject.UnconstructedMembers
                .Select(member => new {
                    Member = member,
                    Setter = _builder.MemberSetters.FirstOrDefault(setter => setter.IsForMember(member, this.Builder, this))
                }).Where(e => e.Setter != null).ToList();
            foreach (var member in unconstructed)
            {
                CurrentObject.RegisterConstructed(member.Member);
            }
            foreach (var member in unconstructed)
            {
                using(PushMember(member.Member))
                {
                    member.Setter.Set(CurrentObject.Object, this.Builder, this);
                }
            }
        }

        private void DoProcessors(Type type)
        {
            foreach (var setter in _builder.Processors.Where(s => s.IsForType(type)).ToArray())
            {
                setter.Process(CurrentObject.Object, this.Builder, this);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            // Take yourself off the Finalization queue
            // to prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if(disposing)
                    IsDisposed = true;
            }
        }
    }
}
