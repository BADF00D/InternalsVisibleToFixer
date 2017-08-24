using System.Collections.Generic;
using System.Linq;

namespace InternalsVisibleToFixer.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Except<T>(this IEnumerable<T> source, T item)
        {
            return source.Except(new[] {item});
        }
    }
}