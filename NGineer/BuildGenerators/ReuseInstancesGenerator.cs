using System;
using System.Linq;
using System.Collections.Generic;
using NGineer.Internal;
using NGineer.Utils;

namespace NGineer.BuildGenerators
{
    public class ReuseInstancesGenerator<T> : IGenerator
    {
        private readonly IEnumerable<ObjectBuildRecord> _collection;

        public ReuseInstancesGenerator(IEnumerable<T> collection)
        {
            _collection = collection.Select(c => new ObjectBuildRecord(typeof(T), c, false));
        }

        public bool GeneratesType(Type type, IBuilder builder, BuildSession session)
        {
            return typeof (T).Equals(type);
        }

        public ObjectBuildRecord CreateRecord(Type type, IBuilder builder, BuildSession session)
        {
            return session.Random.OneFromList(_collection);
        }
    }
}