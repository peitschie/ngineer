using System;
using System.Collections;
using System.Collections.Generic;
using NGineer.Utils;
namespace NGineer.Populators
{
    public class ListPopulator : IPopulator
    {
        public bool PopulatesType(Type type, IBuilder builder, BuildSession session)
        {
            var enumerableType = type.GetEnumerableElementType();
            return enumerableType != null && type.IsAssignableFrom(typeof(List<>).MakeGenericType(enumerableType));
        }

        public void Populate(Type type, object obj, IBuilder builder, BuildSession session)
        {
            var listType = obj.GetType().GetGenericArguments()[0];
            var range = session.GetCollectionSize(listType);
            var list = (IList) obj;
            var listSize = session.Random.NextInRange(range);
            for (int i = 0; i < listSize; i++)
            {
                list.Add(builder.Build(listType, session));
            }
        }
    }
}

