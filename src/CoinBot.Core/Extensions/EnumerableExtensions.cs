using System.Collections.Generic;

namespace CoinBot.Core.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> RemoveNulls<T>(this IEnumerable<T?>? items)
            where T : class
        {
            if (items != null)
            {
                foreach (T? item in items)
                {
                    if (item == null)
                    {
                        continue;
                    }

                    yield return item;
                }
            }
        }
    }
}