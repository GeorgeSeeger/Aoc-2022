using System.Collections.Generic;
using System.Linq;

namespace AoC_2022 {
    public static class EnumerableExtensions {
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> collection, int chunkSize) {
            for (var i = 0; i < collection.Count(); i+= chunkSize) {
                yield return collection.Skip(i).Take(chunkSize);
            }
        }
    }
}