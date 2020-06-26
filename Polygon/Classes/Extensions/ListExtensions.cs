using System.Collections.Generic;
using System.Linq;

namespace Polygon.Classes.Extensions
{
    public static class ListExtensions
    {
        // source: https://stackoverflow.com/a/11246006
        public static List<T> Rotate<T>(this List<T> list, int offset)
        {
            return list.Skip(offset).Concat(list.Take(offset)).ToList();
        }
    }
}
