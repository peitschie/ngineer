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

        public void PushObject(Type type, object obj)
        {
            if(IsDisposed)
                throw new ObjectDisposedException("BuildSession");
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
            if(IsDisposed)
                throw new ObjectDisposedException("BuildSession");
            if(CurrentObject.Parent == null)
                throw new BuilderException("Unable to pop beyond the root element of the built object tree");
            CurrentObject.RequiresPopulation = false;
            CurrentObject = CurrentObject.Parent;
        }

        public void PushMember(MemberInfo property)
        {
            if(IsDisposed)
                throw new ObjectDisposedException("BuildSession");
            _memberStack.Push(property);
            CurrentMember = property;
        }

        public void PopMember(bool valueHasChanged)
        {
            if(IsDisposed)
                throw new ObjectDisposedException("BuildSession");
            var member = _memberStack.Pop();
            CurrentMember = _memberStack.Count > 0 ? _memberStack.Peek() : null;
            if(valueHasChanged)
            {
                CurrentObject.RegisterConstructed(member);
            }
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
            if(IsDisposed)
                throw new ObjectDisposedException("BuildSession");
            if(BuildDepth == _builder.BuildDepth)
            {
                if(_builder.IsBuildDepthUnset || _builder.ThrowWhenBuildDepthReached)
                {
                    throw new DepthExceededException(_builder.BuildDepth, this);
                }
                return type.IsValueType ? Activator.CreateInstance(type) : null;
            }
            if(ConstructedCount > _builder.MaximumObjects)
            {
                throw new MaximumInstancesReachedException(_builder.MaximumObjects, this);
            }

            var generator = _builder.GetGenerator(type, this);
            var obj = generator.Create(type, this.Builder, this);
            if(obj != null)
            {
                PushObject(type, obj);
                obj = CurrentObject.Object;
                if(CurrentObject.RequiresPopulation)
                {
                    DoMemberSetters();
                    if(!_builder.ShouldIgnoreUnset(type))
                    {
                        generator.Populate(type, obj, this.Builder, this);
                    }
                    DoPopulators(type);
                }
                PopObject();
            }
            return obj;
        }

        private void DoMemberSetters()
        {
            foreach(var member in CurrentObject.UnconstructedMembers)
            {
                PushMember(member);
                var setter = _builder.MemberSetters.FirstOrDefault(s => s.IsForMember(member, this.Builder, this));
                if(setter != null)
                    setter.Set(CurrentObject.Object, this.Builder, this);
                PopMember(setter != null);
            }
        }

        private void DoPopulators(Type type)
        {
            foreach (var setter in _builder.Setters.Where(s => s.IsForType(type)).ToArray())
            {
                setter.Set(CurrentObject.Object, this.Builder, this);
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
