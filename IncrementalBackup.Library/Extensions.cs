using System;
using System.Collections.Generic;

namespace IncrementalBackup.Library
{
    public static class Extensions
    {
        public static void AddRange<T>(this HashSet<T> hashSet, IEnumerable<T> items)
        {
            if (hashSet == null) throw new ArgumentNullException("hashSet");
            if (items == null) throw new ArgumentNullException("items");

            foreach (var item in items)
            {
                hashSet.Add(item);
            }
        }
    }
}
