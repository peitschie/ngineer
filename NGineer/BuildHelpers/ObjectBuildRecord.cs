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
        private readonly List<string> _constructedMembers = new List<string>();
        private readonly List<MemberInfo> _unconstructedMembers;
        private bool _requiresPopulation;

        public ObjectBuildRecord(Type type, object obj)
        {
            _type = type;
            _obj = obj;
            _unconstructedMembers = new List<MemberInfo>();
            _requiresPopulation = true;
            if (_obj != null && !_type.IsAssignableFrom(_obj.GetType()))
            {
                throw new InvalidCastException("Object type {0} is not equivalent to passed in type {1}".With(obj.GetType(), type));
            }
            var objType = obj != null ? obj.GetType() : type;
            
            _unconstructedMembers.AddRange(objType.GetProperties()
                .Where(p => p.CanWrite)
                .OrderBy(p => p.Name).Cast<MemberInfo>());
            _unconstructedMembers.AddRange(objType.GetFields()
                .Where(f => f.IsPublic && !f.IsLiteral && !f.IsInitOnly && !f.IsLiteral)
                .OrderBy(p => p.Name).Cast<MemberInfo>());
        }

        public Type Type { get { return _type; } }
        public object Object { get { return _obj; } }

        public bool IsConstructed(MemberInfo property)
        {
            return _constructedMembers.Contains(property.Name);
        }

        public bool RequiresPopulation
        {
            get
            {
                return _requiresPopulation;
            }
            set
            {
                if (value && !_requiresPopulation)
                {
                    throw new InvalidOperationException("A populated object cannot be set back to unpopulated");
                }
                _requiresPopulation = value;
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