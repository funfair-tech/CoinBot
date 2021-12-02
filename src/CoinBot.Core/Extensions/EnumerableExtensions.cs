using System.Collections.Generic;

namespace CoinBot.Core.Extensions;

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

    /// <summary>
    ///     Splits the enumerable into a set of n-itemed lists.
    /// </summary>
    /// <param name="source">The source to split</param>
    /// <param name="splitSize">the number of items to splt</param>
    /// <typeparam name="T">Type of the element</typeparam>
    /// <returns>Set of items.</returns>
    public static IEnumerable<IReadOnlyList<T>> Split<T>(this IEnumerable<T> source, int splitSize)
    {
        List<T> items = new(splitSize);

        foreach (T entry in source)
        {
            if (items.Count == splitSize)
            {
                yield return items;

                items = new(splitSize);
            }

            items.Add(entry);
        }

        if (items.Count != 0)
        {
            yield return items;
        }
    }
}