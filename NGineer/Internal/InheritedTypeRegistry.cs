using System;

namespace NGineer.Internal
{
    public class InheritedTypeRegistry<TStoreType> : ITypeRegistry<TStoreType>
    {
        private readonly ITypeRegistry<TStoreType> _parent;
        private readonly ITypeRegistry<TStoreType> _current;

        public InheritedTypeRegistry(ITypeRegistry<TStoreType> parent, ITypeRegistry<TStoreType> current)
        {
            _parent = parent;
            _current = current;
        }

		public bool HasType(Type type)
		{
			return _current.HasType(type) || (_parent != null && _parent.HasType(type));	
		}
		
        public TStoreType GetForType(Type type)
        {
			if(_current.HasType(type))
			{
				return _current.GetForType(type);
			}
			if(_parent != null && _parent.HasType(type))
			{
				return _parent.GetForType(type);
			}
			return default(TStoreType);
        }
    }
}