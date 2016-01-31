using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    public static class ListShuffle
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            int count = list.Count;
            Random rnd = new Random();
            while (count > 1)
            {
                int k = (rnd.Next(0, count) % count);
                count--;
                T value = list[k];
                list[k] = list[count];
                list[count] = value;
            }
        }

        public static void ShuffleFromIndex<T>(this List<T> list, int index)
        {
            List<T> shuffled = new List<T>(list.Count - index + 1);
            List<T> unshuffled = new List<T>(index + 1);

            for (int i = 0; i < list.Count; i++)
            {
                if (i < index)
                    unshuffled.Add(list[i]);
                else
                    shuffled.Add(list[i]);
            }

            shuffled.Shuffle();

            list.Clear();

            list.AddRange(unshuffled);
            list.AddRange(shuffled);
        }
    }
}
