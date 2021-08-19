using System;
using System.Collections.Generic;

namespace RP
{
    public static class IListExtensions
    {
        private static readonly Random Rnd = new Random();

        public static bool TryPickRandom<T>(this IList<T> list, out T element)
        {
            if (list.Count <= 0)
            {
                element = default;
                return false;
            }

            element = PickRandom(list);
            return true;
        }

        public static T PickRandom<T>(this IList<T> list)
        {
            return PickRandom(list, list.Count);
        }

        public static T PickRandom<T>(this IList<T> list, int count)
        {
            var r = Rnd.Next(count);
            return list[r];
        }
    }
}