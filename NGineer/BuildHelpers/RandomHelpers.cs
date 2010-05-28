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
		
		public static IList<TType> Shuffle<TType>(IEnumerable<TType> chooseList, Random random)
		{
			var list = new List<TType>();
			Shuffle(list, chooseList, random);
			return list;
		}
		
		public static void Shuffle<TType>(IList<TType> list, IEnumerable<TType> chooseList, Random random)
		{
			var entries = new List<TType>(chooseList);
			while(entries.Count > 0)
			{
				var nextIndex = random.Next(entries.Count);
				list.Add(entries[nextIndex]);
				entries.RemoveAt(nextIndex);
			}	
		}
    }
}