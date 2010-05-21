using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NGineer.Utils;

namespace NGineer.BuildHelpers
{
    public class ObjectBuildRecord : IObjectBuildRecord
    {
        private readonly Type _type;
        private readonly object _obj;
        private readonly HashSet<string> _constructedMembers = new HashSet<string>();
        private readonly List<MemberInfo> _unconstructedMembers;

        private bool _isPopulated;

        public ObjectBuildRecord(Type type, object obj)
        {
            _type = type;
            _obj = obj;
            _unconstructedMembers = new List<MemberInfo>();
            if(_obj != null && !_type.IsAssignableFrom(_obj.GetType()))
            {
                throw new InvalidCastException("Object type {0} is not equivalent to passed in type {1}".With(obj.GetType(), type));
            }
            if (obj != null)
            {
                
                _unconstructedMembers.AddRange(obj.GetType().GetProperties());
                _unconstructedMembers.AddRange(obj.GetType().GetFields());
            }
            else
            {
                _unconstructedMembers.AddRange(type.GetProperties());
                _unconstructedMembers.AddRange(type.GetFields());
            }
        }

        public Type Type { get { return _type; } }
        public object Object { get { return _obj; } }

        public bool IsConstructed(MemberInfo property)
        {
            return _constructedMembers.Contains(property.Name);
        }

        public bool IsPopulated
        {
            get
            {
                return _isPopulated;
            }
            set
            {
                if (_isPopulated && !value)
                {
                    throw new InvalidOperationException("A populated object cannot be set back to unpopulated");
                }
                _isPopulated = value;
            }
        }

        public MemberInfo[] UnconstructedMembers { get { return _unconstructedMembers.ToArray(); } }

        public void RegisterConstructed(MemberInfo member)
        {
            var existing = _unconstructedMembers.First(p => p.Name == member.Name);
            _constructedMembers.Add(member.Name);
            _unconstructedMembers.Remove(existing);
        }
    }
}