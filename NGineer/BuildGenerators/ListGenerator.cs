using System;
using System.Collections;
using System.Collections.Generic;
using NGineer.Internal;
using NGineer.Utils;

namespace NGineer.BuildGenerators
{
    public class ListGenerator : IGenerator
    {
        public bool GeneratesType(Type type, IBuilder builder, BuildSession session)
        {
            var enumerableType = type.GetEnumerableElementType();
            return enumerableType != null && type.IsAssignableFrom(typeof(List<>).MakeGenericType(enumerableType));
        }

        public object Create(Type type, IBuilder builder, BuildSession session)
        {
            var listType = type.GetGenericArguments()[0];
            var constructorType = typeof (List<>).MakeGenericType(listType);
            var constructor = constructorType.GetConstructor(new Type[0]);
            if (constructor == null)
            {
                throw new BuilderException(string.Format("Unable to construct {0} as no default constructor was found", type));
            }

            var list = (IList) constructor.Invoke(new object[0]);

            return list;
        }

        public void Populate(Type type, object obj, IBuilder builder, BuildSession session)
        {
            var listType = obj.GetType().GetGenericArguments()[0];
            var range = session.GetCollectionSize(listType);
            var list = (IList) obj;
            var listSize = session.Random.NextInRange(range);
            for (int i = 0; i < listSize; i++)
            {
                list.Add(builder.Build(listType));
            }
        }
    }
}