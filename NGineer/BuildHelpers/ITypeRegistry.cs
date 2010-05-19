using System;

namespace NGineer.BuildHelpers
{
    public interface ITypeRegistry<TStoreType>
    {
        TStoreType GetForType(Type type);
    }
}