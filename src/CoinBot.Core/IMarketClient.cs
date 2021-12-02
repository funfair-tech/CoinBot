using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoinBot.Core;

public interface IMarketClient
{
    string Name { get; }

    /// <summary>
    ///     Gets the market summaries
    /// </summary>
    /// <returns></returns>
    Task<IReadOnlyCollection<MarketSummaryDto>> GetAsync(ICoinBuilder builder);
}