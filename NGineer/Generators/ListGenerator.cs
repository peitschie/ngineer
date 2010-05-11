using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NGineer.BuildHelpers;
using NGineer.Utils;

namespace NGineer.Generators
{
    public class ListGenerator : IGenerator, ICollectionGenerator
    {
        private readonly Random _random;

        public ListGenerator(int seed, int minListItems, int maxListItems)
        {
            MinimumListItems = minListItems;
            MaximumListItems = maxListItems;
            _random = new Random(seed);
        }
		
		public int MinimumListItems { get; set; }
		public int MaximumListItems { get; set; }

        public bool GeneratesType(Type type, IBuilder builder, BuildSession session)
        {
            return typeof (IList<>).IsGenericAssignableFrom(type) 
                || typeof(IEnumerable<>).IsGenericAssignableFrom(type);
        }

        public object Create(Type type, IBuilder builder, BuildSession session)
        {
            var listType = type.GetGenericArguments()[0];
            var constructorType = typeof (List<>).MakeGenericType(listType);
            var constructor = constructorType.GetConstructor(new Type[0]);
            if (constructor == null)
            {
                throw new BuilderException("Unable to construct {0} as no default constructor was found".With(type));
            }

            var list = (IList) constructor.Invoke(new object[0]);

            return list;
        }

        public object Populate(Type type, object obj, IBuilder builder, BuildSession session)
        {
            var listType = obj.GetType().GetGenericArguments()[0];
            var list = (IList) obj;
            var listSize = MinimumListItems + _random.Next(MaximumListItems - MinimumListItems);
            for (int i = 0; i < listSize; i++)
            {
                list.Add(builder.Build(listType, session));
            }
            return list;
        }
    }
}