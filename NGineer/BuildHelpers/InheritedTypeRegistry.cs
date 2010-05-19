using System;

namespace NGineer.BuildHelpers
{
    public class InheritedTypeRegistry<TStoreType> : ITypeRegistry<TStoreType>
        where TStoreType : class
    {
        private readonly ITypeRegistry<TStoreType> _parent;
        private readonly ITypeRegistry<TStoreType> _current;

        public InheritedTypeRegistry(ITypeRegistry<TStoreType> parent, ITypeRegistry<TStoreType> current)
        {
            _parent = parent;
            _current = current;
        }

        public TStoreType GetForType(Type type)
        {
            return _current.GetForType(type) ?? (_parent != null ? _parent.GetForType(type) : default(TStoreType));
        }
    }
}