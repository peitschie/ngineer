using System;
using System.Collections;
using System.Collections.Generic;
using NGineer.Internal;
using NGineer.Utils;
using NGineer.Exceptions;

namespace NGineer.BuildGenerators
{
    public class ListGenerator : IGenerator
    {
        public bool GeneratesType(Type type, IBuilder builder, BuildSession session)
        {
            var enumerableType = type.GetEnumerableElementType();
            return enumerableType != null && type.IsAssignableFrom(typeof(List<>).MakeGenericType(enumerableType));
        }

        public ObjectBuildRecord CreateRecord(Type type, IBuilder builder, BuildSession session)
        {
            var listType = type.GetGenericArguments()[0];
            var constructorType = typeof(List<>).MakeGenericType(listType);
            var constructor = constructorType.GetConstructor(Type.EmptyTypes);
            if (constructor == null)
            {
                throw new BuilderException(string.Format("Unable to construct {0} as no default constructor was found", type));
            }

            var list = (IList)constructor.Invoke(new object[0]);

            if (session.AvailableBuildDepth >= 1)
            {
                var range = session.GetCollectionSize(listType);
                var listSize = session.Random.NextInRange(range);
                for (int i = 0; i < listSize; i++)
                {
                    list.Add(builder.Build(listType));
                }
            }

            return new ObjectBuildRecord(type, list, false);
        }
    }
}