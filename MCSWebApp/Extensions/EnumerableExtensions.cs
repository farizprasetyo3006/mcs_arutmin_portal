using System.Collections.Generic;

namespace MCSWebApp.Extensions
{
    public static class EnumerableExtensions
    {
        public static List<T> ToSafeList<T>(this IEnumerable<T> source) => new List<T>(source);
    }
}