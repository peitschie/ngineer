using System;
using System.Collections.Generic;
using System.Linq;

namespace NGineer.BuildHelpers
{
    public class TypeRegistry<TStoreType> : ITypeRegistry<TStoreType>
    {
        private readonly HashSet<TypeRegistryEntry> _entries = new HashSet<TypeRegistryEntry>();

        public TStoreType GetForType(Type type)
        {
            var entry = GetEntryForType(type);
            return entry != null ? entry.Value : default(TStoreType);
        }
		
		public bool HasType(Type type)
		{
			return _entries.Count(t => t.IsForType(type)) > 0;
		}
		
		public bool HasType<TType>()
		{
			return HasType(typeof(TType));
		}

        private TypeRegistryEntry GetEntryForType(Type type)
        {
            return _entries.FirstOrDefault(t => t.IsForType(type));
        }

        public TStoreType GetForType<TType>()
        {
            return GetForType(typeof (TType));
        }

        public void SetForType(Type type, TStoreType value)
        {
            var newEntry = new TypeRegistryEntry(type, value);
            _entries.Remove(newEntry);
            _entries.Add(newEntry);
        }

        public void SetForType<TType>(TStoreType value)
        {
            SetForType(typeof(TType), value);
        }

        private class TypeRegistryEntry
        {
            private readonly Type _type;
            private readonly TStoreType _value;

            public TypeRegistryEntry(Type type, TStoreType value)
            {
                _type = type;
                _value = value;
            }

            public TStoreType Value
            {
                get { return _value; }
            }

            public bool IsForType(Type type)
            {
                return _type.IsAssignableFrom(type);
            }

            private bool Equals(TypeRegistryEntry other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(other._type, _type);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof (TypeRegistryEntry)) return false;
                return Equals((TypeRegistryEntry) obj);
            }

            public override int GetHashCode()
            {
                return (_type != null ? _type.GetHashCode() : 0);
            }
        }
    }
}