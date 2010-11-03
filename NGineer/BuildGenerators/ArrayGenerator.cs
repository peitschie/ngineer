using System;
using NGineer.BuildHelpers;
using NGineer.Utils;

namespace NGineer.BuildGenerators
{
    public class ArrayGenerator : IGenerator
    {
        public bool GeneratesType(Type type, IBuilder builder, BuildSession session)
        {
            return type.IsArray;
        }

        public object Create(Type type, IBuilder builder, BuildSession session)
        {
            var arrayType = type.GetElementType();
            var range = session.GetCollectionSize(arrayType);
            return Array.CreateInstance(arrayType, session.Random.NextInRange(range));
        }
    }
}