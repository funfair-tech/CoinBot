using System.Collections.Generic;
using System.Threading.Tasks;
using CoinBot.Core.Extensions;

namespace CoinBot.Core.Helpers;

public static class Batched
{
    public static async Task<IReadOnlyList<TResult>> WhenAllAsync<TResult>(int concurrent, IEnumerable<Task<TResult>> items)
    {
        List<TResult> results = new();

        foreach (IReadOnlyList<Task<TResult>> batch in items.Split(concurrent))
        {
            TResult[] intermediate = await Task.WhenAll(batch);
            results.AddRange(intermediate);
        }

        return results;
    }
}