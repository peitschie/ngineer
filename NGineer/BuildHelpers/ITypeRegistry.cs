using System;

namespace NGineer.BuildHelpers
{
    public interface ITypeRegistry<TStoreType>
    {
		bool HasType(Type type);
        TStoreType GetForType(Type type);
    }
}