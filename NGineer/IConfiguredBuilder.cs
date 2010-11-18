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
        IEnumerable<IInstanceProcessor> Processors { get; }
        IEnumerable<IMemberSetter> MemberSetters { get; }

        IGenerator GetGenerator(Type type, BuildSession session);

        bool ShouldIgnoreUnset(Type type);
        IConfiguredBuilder CreateNew(BuildSession session);
    }
}

