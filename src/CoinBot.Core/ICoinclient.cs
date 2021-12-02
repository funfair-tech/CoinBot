using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoinBot.Core;

public interface ICoinClient
{
    /// <summary>
    ///     Get all coin info.
    /// </summary>
    /// <returns></returns>
    Task<IReadOnlyCollection<ICoinInfo>> GetCoinInfoAsync();

    /// <summary>
    ///     Get global info.
    /// </summary>
    /// <returns></returns>
    Task<IGlobalInfo?> GetGlobalInfoAsync();
}