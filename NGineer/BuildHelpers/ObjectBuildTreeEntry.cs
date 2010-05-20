using System;
using System.Collections.Generic;
using System.Reflection;
using NGineer.Utils;

namespace NGineer.BuildHelpers
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
            get { return _record.Type; }
        }

        public object Object
        {
            get { return _record != null ? _record.Object : null; }
        }

        public PropertyInfo[] UnconstructedProperties
        {
            get { return _record.UnconstructedProperties; }
        }

        public FieldInfo[] UnconstructedFields
        {
            get { return _record.UnconstructedFields; }
        }

        public bool IsPopulated
        {
            get { return _record.IsPopulated; }
            set { _record.IsPopulated = value; }
        }

        public bool IsConstructed(MemberInfo property)
        {
            return _record.IsConstructed(property);
        }

        public void RegisterConstructed(MemberInfo member)
        {
			_record.RegisterConstructed(member);
        }
    }
}