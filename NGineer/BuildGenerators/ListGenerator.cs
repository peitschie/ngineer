using System;
using System.Collections;
using System.Collections.Generic;
using NGineer.BuildHelpers;
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
                throw new BuilderException("Unable to construct {0} as no default constructor was found".With(type));
            }

            var list = (IList) constructor.Invoke(new object[0]);

            return list;
        }
    }
}