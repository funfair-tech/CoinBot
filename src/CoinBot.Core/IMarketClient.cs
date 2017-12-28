using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoinBot.Core
{
	public interface IMarketClient
	{
		string Name { get; }

		/// <summary>
		/// TODO
		/// </summary>
		/// <returns></returns>
		Task<IReadOnlyCollection<MarketSummaryDto>> Get();
	}
}
