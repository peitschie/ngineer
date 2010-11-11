using System;
using System.Collections.Generic;
using NGineer.Internal;
using NGineer.Utils;

namespace NGineer.BuildGenerators
{
    public class ReuseInstancesGenerator<T> : IGenerator
    {
        private readonly IEnumerable<T> _collection;

        public ReuseInstancesGenerator(IEnumerable<T> collection)
        {
            _collection = collection;
        }

        public bool GeneratesType(Type type, IBuilder builder, BuildSession session)
        {
            return typeof (T).Equals(type);
        }

        public object Create(Type type, IBuilder builder, BuildSession session)
        {
            return session.Random.OneFromList(_collection);
        }

        public void Populate(Type type, object obj, IBuilder builder, BuildSession session)
        {
        }
    }
}