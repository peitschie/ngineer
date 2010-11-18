using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NGineer.Utils;

namespace NGineer.Internal
{
    public class ObjectBuildRecord : IObjectBuildRecord
    {
        private readonly Type _type;
        private readonly object _obj;
        private readonly List<string> _constructedMembers = new List<string>();
        private readonly List<MemberInfo> _unconstructedMembers;

        public ObjectBuildRecord(Type type, object obj, bool requiresPopulation)
        {
            _type = type;
            _obj = obj;
            _unconstructedMembers = new List<MemberInfo>();
            if (_obj != null && !_type.IsAssignableFrom(_obj.GetType()))
            {
                throw new InvalidCastException(string.Format("Object type {0} is not equivalent to passed in type {1}", obj.GetType(), type));
            }

            if (requiresPopulation)
            {
                var objType = obj != null ? obj.GetType() : type;

                _unconstructedMembers.AddRange(objType.GetProperties()
                    .Where(p => p.CanWrite)
                    .OrderBy(p => p.Name).Cast<MemberInfo>());
                _unconstructedMembers.AddRange(objType.GetFields()
                    .Where(f => f.IsPublic && !f.IsLiteral && !f.IsInitOnly && !f.IsLiteral)
                    .OrderBy(p => p.Name).Cast<MemberInfo>());
            }
        }

        public Type Type { get { return _type; } }
        public object Object { get { return _obj; } }

        public bool Counted { get;set; }

        public MemberInfo[] UnconstructedMembers { get { return _unconstructedMembers.ToArray(); } }

        public void RegisterConstructed(MemberInfo member)
        {
            var existing = _unconstructedMembers.First(p => p.Name == member.Name);
            _constructedMembers.Add(member.Name);
            _unconstructedMembers.Remove(existing);
        }
    }
}