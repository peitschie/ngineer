using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NGineer.BuildHelpers
{
    public static class RandomHelpers
    {
        public static object OneFromList(IList list, Random random)
        {
            return list[random.Next(list.Count)];
        }

        public static object OneFromList<TType>(IEnumerable<TType> list, Random random)
        {
            return list.ElementAt(random.Next(list.Count()));
        }
    }
}