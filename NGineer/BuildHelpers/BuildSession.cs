using System;
using System.Collections.Generic;
using System.Linq;
using NGineer.Generators;
using NGineer.Utils;

namespace NGineer.BuildHelpers
{
    public class BuildSession
    {
        public int BuildDepth { get; set; }

        private readonly HashSet<object> _constructedMembers = new HashSet<object>();
        public HashSet<object> ConstructedMembers { get { return _constructedMembers; } }

        private readonly List<object> _constructedObjects = new List<object>();
        public IList<object> ConstructedObjects { get { return _constructedObjects; } }

        public int NumberOfInstances<TType>()
        {
            return ConstructedObjects.Count(o => o is TType);
        }
    }
}