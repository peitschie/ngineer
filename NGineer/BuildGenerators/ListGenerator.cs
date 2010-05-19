using System;
using System.Collections;
using System.Collections.Generic;
using NGineer.BuildHelpers;
using NGineer.Utils;

namespace NGineer.BuildGenerators
{
    public class ListGenerator : IGenerator
    {
        private readonly Random _random;

        public ListGenerator(int seed)
        {
            _random = new Random(seed);
        }
		
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

        public void Populate(Type type, object obj, IBuilder builder, BuildSession session)
        {
            var listType = obj.GetType().GetGenericArguments()[0];
            var range = session.GetCollectionSize(listType);
            var list = (IList) obj;
            var listSize = range.Minimum + _random.Next(range.Maximum - range.Minimum);
            for (int i = 0; i < listSize; i++)
            {
                list.Add(builder.Build(listType, session));
            }
        }
    }
}