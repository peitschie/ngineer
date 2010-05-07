using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NGineer.BuildHelpers;
using NGineer.Utils;

namespace NGineer.Generators
{
    public class ListGenerator : IGenerator
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

        public bool GeneratesType(Type type, IBuilder builder)
        {
            return typeof (IList<>).IsGenericAssignableFrom(type);
        }

        public object Generate(Type type, IBuilder builder)
        {
            var listType = type.GetGenericArguments()[0];
            var constructorType = typeof (List<>).MakeGenericType(listType);
            var constructor = constructorType.GetConstructor(new Type[0]);
            if(constructor == null)
            {
                throw new BuilderException("Unable to construct {0} as no default constructor was found".With(type));
            }

            var list = (IList)constructor.Invoke(new object[0]);
            var listSize = MinimumListItems + _random.Next(MaximumListItems - MinimumListItems);
            for(int i = 0; i < listSize; i++)
            {
                list.Add(builder.Build(listType));
            }

            return list;
        }

        public object Generate(PropertyInfo property, IBuilder builder)
        {
            return Generate(property.PropertyType, builder);
        }

        public object Generate(FieldInfo field, IBuilder builder)
        {
            return Generate(field.FieldType, builder);
        }
    }
}