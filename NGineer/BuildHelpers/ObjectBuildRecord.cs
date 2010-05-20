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
        private readonly List<PropertyInfo> _unconstructedProperties;
        private readonly List<FieldInfo> _unconstructedFields;

        private bool _isPopulated;

        public ObjectBuildRecord(Type type, object obj)
        {
            _type = type;
            _obj = obj;
            if(_obj != null && !_type.IsAssignableFrom(_obj.GetType()))
            {
                throw new InvalidCastException("Object type {0} is not equivalent to passed in type {1}".With(obj.GetType(), type));
            }
            if (obj != null)
            {
                _unconstructedProperties = new List<PropertyInfo>(obj.GetType().GetProperties());
                _unconstructedFields = new List<FieldInfo>(obj.GetType().GetFields());
            }
            else
            {
                _unconstructedProperties = new List<PropertyInfo>(type.GetProperties());
                _unconstructedFields = new List<FieldInfo>(type.GetFields());
            }
        }

        public Type Type { get { return _type; } }
        public object Object { get { return _obj; } }

        public PropertyInfo[] UnconstructedProperties { get { return _unconstructedProperties.ToArray(); } }
        public FieldInfo[] UnconstructedFields { get { return _unconstructedFields.ToArray(); } }

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

		public void RegisterConstructed(MemberInfo member)
        {
			switch(member.MemberType)
			{
				case MemberTypes.Field:
					RegisterConstructed(member as FieldInfo);
					break;
				case MemberTypes.Property:
					RegisterConstructed(member as PropertyInfo);
					break;
				default:
					throw new BuilderException("Unsupported member type {0}".With(member.MemberType));
			}
        }
		
        private void RegisterConstructed(PropertyInfo property)
        {
            var existing = _unconstructedProperties.First(p => p.Name == property.Name);
            _constructedMembers.Add(property.Name);
            _unconstructedProperties.Remove(existing);
        }

        private void RegisterConstructed(FieldInfo field)
        {
            var existing = _unconstructedFields.First(p => p.Name == field.Name);
            _constructedMembers.Add(field.Name);
            _unconstructedFields.Remove(existing);
        }
    }
}