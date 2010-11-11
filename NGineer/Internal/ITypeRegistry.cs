using System;

namespace NGineer.Internal
{
    public interface ITypeRegistry<TStoreType>
    {
		bool HasType(Type type);
        TStoreType GetForType(Type type);
    }
}