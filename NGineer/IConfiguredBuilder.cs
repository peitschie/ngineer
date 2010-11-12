using System;
using NGineer.Internal;
using System.Collections.Generic;
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

        IEnumerable<Action<BuildSession>> PostBuildHooks { get; }
        IEnumerable<IInstanceProcessor> Setters { get; }
        IEnumerable<IMemberSetter> MemberSetters { get; }

        bool ShouldIgnoreUnset(Type type);
        IGenerator GetGenerator(Type type, BuildSession session);
    }
}

