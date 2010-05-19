using System;
using NGineer.BuildHelpers;

namespace NGineer.BuildGenerators
{
    public class ArrayGenerator : IGenerator
    {
        private readonly Random _random;

        public ArrayGenerator(int seed)
        {
            _random = new Random(seed);
        }
		
        public bool GeneratesType(Type type, IBuilder builder, BuildSession session)
        {
            return type.IsArray;
        }

        public object Create(Type type, IBuilder builder, BuildSession session)
        {
            var arrayType = type.GetElementType();
            var range = session.GetCollectionSize(arrayType);
            return Array.CreateInstance(arrayType, range.Minimum + _random.Next(range.Maximum - range.Minimum));
        }

        public void Populate(Type type, object obj, IBuilder builder, BuildSession session)
        {
            var arrayType = type.GetElementType();
            var array = (Array) obj;
            for (int i = 0; i < array.Length; i++)
            {
                array.SetValue(builder.Build(arrayType, session), i);
            }
        }
    }
}