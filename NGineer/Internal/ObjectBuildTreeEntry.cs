using System;
using System.Collections.Generic;
using System.Reflection;
using NGineer.Exceptions;

namespace NGineer.Internal
{
    public class ObjectBuildTreeEntry : IObjectBuildRecord
    {
        private readonly ObjectBuildTreeEntry _parent;
        private readonly List<ObjectBuildTreeEntry> _children = new List<ObjectBuildTreeEntry>();
        private readonly ObjectBuildRecord _record;
        private readonly int _depth;

        public ObjectBuildTreeEntry(ObjectBuildTreeEntry parent, ObjectBuildRecord record, int depth)
        {
            _parent = parent;
            _record = record;
            _depth = depth;
        }

        public ObjectBuildTreeEntry AddChild(ObjectBuildRecord child)
        {
            var newEntry = new ObjectBuildTreeEntry(this, child, _depth + 1);
            _children.Add(newEntry);
            return newEntry;
        }

        public ObjectBuildTreeEntry Parent
        {
            get { return _parent; }
        }

        public List<ObjectBuildTreeEntry> Children
        {
            get { return _children; }
        }

        public ObjectBuildRecord Record
        {
            get { return _record; }
        }

        public int Depth
        {
            get { return _depth; }
        }

        public Type Type
        {
            get { return _record == null ? null : _record.Type; }
        }

        public object Object
        {
            get { return _record == null ? null : _record.Object; }
        }

        public MemberInfo[] UnconstructedMembers
        {
            get { return _record == null ? new MemberInfo[0] : _record.UnconstructedMembers; }
        }

        public void RegisterConstructed(MemberInfo member)
        {
            if(_record == null)
                throw new BuilderException("Attempting to construct member to non-existent build record");
			_record.RegisterConstructed(member);
        }
    }
}