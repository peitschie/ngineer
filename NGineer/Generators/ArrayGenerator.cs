using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NGineer.BuildHelpers;
using NGineer.Utils;

namespace NGineer.Generators
{
    public class ArrayGenerator : IGenerator, ICollectionGenerator
    {
        private readonly Random _random;

        public ArrayGenerator(int seed, int minListItems, int maxListItems)
        {
            MinimumListItems = minListItems;
            MaximumListItems = maxListItems;
            _random = new Random(seed);
        }
		
		public int MinimumListItems { get; set; }
		public int MaximumListItems { get; set; }

        public bool GeneratesType(Type type, IBuilder builder, BuildSession session)
        {
            return type.IsArray;
        }

        public object Create(Type type, IBuilder builder, BuildSession session)
        {
            var arrayType = type.GetElementType();
            return Array.CreateInstance(arrayType, MinimumListItems + _random.Next(MaximumListItems - MinimumListItems));
        }

        public object Populate(Type type, object obj, IBuilder builder, BuildSession session)
        {
            var arrayType = type.GetElementType();
            var array = (Array) obj;
            for (int i = 0; i < array.Length; i++)
            {
                array.SetValue(builder.Build(arrayType, session), i);
            }
            return array;
        }
    }
}