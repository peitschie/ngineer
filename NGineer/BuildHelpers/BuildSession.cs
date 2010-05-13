using System;
using System.Collections.Generic;
using System.Linq;
using NGineer.Generators;
using NGineer.Utils;

namespace NGineer.BuildHelpers
{
    public class BuildSession
    {
        private readonly TypeRegistry<Range> _collectionSizes;
        private readonly Range _defaultCollectionSize;

        public BuildSession(TypeRegistry<Range> collectionSizes, Range defaultCollectionSize)
        {
            _collectionSizes = collectionSizes;
            _defaultCollectionSize = defaultCollectionSize;
        }

        private readonly HashSet<object> _constructedMembers = new HashSet<object>();
        private readonly List<object> _constructedObjects = new List<object>();
        
        public HashSet<object> ConstructedMembers { get { return _constructedMembers; } }
        public IList<object> ConstructedObjects { get { return _constructedObjects; } }

        public int BuildDepth { get; set; }

        public int NumberOfInstances<TType>()
        {
            return ConstructedObjects.Count(o => o is TType);
        }

        public Range GetCollectionSize(Type type)
        {
            return _collectionSizes.GetForType(type) ?? _defaultCollectionSize;
        }
    }
}