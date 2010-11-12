using System;
using NGineer.Internal;
namespace NGineer
{
    public interface IConfiguredBuilder : IBuilder
    {
        bool ThrowWhenBuildDepthReached { get; }
        bool IsBuildDepthUnset { get; }
        int MaximumObjects { get; }
        Range DefaultCollectionSize { get; }
        ITypeRegistry<Range> CollectionSizes { get; }
        ITypeRegistry<int?> MaxInstances { get; }
    }
}

