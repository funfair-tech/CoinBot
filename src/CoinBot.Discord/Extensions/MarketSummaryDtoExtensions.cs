using CoinBot.Core;
using CoinBot.Core.Extensions;

namespace CoinBot.Discord.Extensions
{
	/// <summary>
	/// <see cref="MarketSummaryDto"/> extension methods.
	/// </summary>
	public static class MarketSummaryDtoExtensions
	{
		public static string GetSummary(this MarketSummaryDto market) => $"{market.BaseCurrrency.Symbol}/{market.MarketCurrency.Symbol}: {market.Last.AsPrice()} (Vol.: {market.Volume.AsVolume()})";
	}
}
