using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NGineer.Internal;

namespace NGineer
{
    public static class RandomExtensions
    {
        public static object OneFromList(this Random random, IList list)
        {
            return list[random.Next(list.Count)];
        }

        public static TType OneFromList<TType>(this Random random, IEnumerable<TType> list)
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

        /// <summary>
        ///
        /// </summary>
        /// <param name="random">
        /// A <see cref="Random"/>
        /// </param>
        /// <param name="min">
        /// A <see cref="System.Int32"/>
        /// Inclusive lower bound of Range
        /// </param>
        /// <param name="max">
        /// A <see cref="System.Int32"/>
        /// Inclusive upper bound of Range
        /// </param>
        /// <returns>
        /// A <see cref="System.Int32"/>
        /// </returns>
		public static int NextInRange(this Random random, int min, int max)
		{
			return random.Next(max - min + 1) + min;
		}
		
		public static int NextInRange(this Random random, Range range)
		{
			return NextInRange(random, range.Minimum, range.Maximum);
		}
	}
}