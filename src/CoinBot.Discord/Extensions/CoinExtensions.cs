using System.Text;
using CoinBot.CoinSources;

namespace CoinBot.Discord.Extensions
{
	/// <summary>
	/// <see cref="ICoin"/> extension methods.
	/// </summary>
	public static class CoinExtensions
	{
		/// <summary>
		/// The USD precision to use when formatting <see cref="ICoin"/> prices.
		/// </summary>
		private const int UsdPricePrecision = 7;

		/// <summary>
		/// Get information about the price change from last hour, day and week.
		/// </summary>
		/// <param name="coin">The <see cref="ICoin"/>.</param>
		/// <returns></returns>
		public static string GetChange(this ICoin coin)
		{
			var changeStringBuilder = new StringBuilder();
			changeStringBuilder.AppendLine($"Hour: {coin.HourChange.AsPercentage()}");
			changeStringBuilder.AppendLine($"Day: {coin.DayChange.AsPercentage()}");
			changeStringBuilder.AppendLine($"Week: {coin.WeekChange.AsPercentage()}");
			return changeStringBuilder.ToString();
		}

		/// <summary>
		/// Get a summary about the price change from last hour, day and week.
		/// </summary>
		/// <param name="coin"></param>
		/// <returns></returns>
		public static string GetChangeSummary(this ICoin coin) => $"{coin.HourChange.AsPercentage()} | {coin.DayChange.AsPercentage()} | {coin.WeekChange.AsPercentage()}";

		/// <summary>
		/// Get the coin image url.
		/// TODO: Move to a CoinMarketCap specific location?
		/// </summary>
		/// <param name="coin"></param>
		/// <returns></returns>
		public static string GetCoinImageUrl(this ICoin coin) => $"https://files.coinmarketcap.com/static/img/coins/64x64/{coin.Id}.png";

		/// <summary>
		/// Get the coin market cap link.
		/// TODO: Move to a CoinMarketCap specific location?
		/// </summary>
		/// <param name="coin"></param>
		/// <returns></returns>
		public static string GetCoinMarketCapLink(this ICoin coin) => $"https://coinmarketcap.com/currencies/{coin.Id}/";

		/// <summary>
		/// Get the <paramref name="coin"/> description, including market cap, rank and 24H volume.
		/// </summary>
		/// <param name="coin">The <see cref="ICoin"/>.</param>
		/// <returns></returns>
		public static string GetDescription(this ICoin coin)
		{
			var descriptionBuilder = new StringBuilder();
			descriptionBuilder.AppendLine($"Market cap {coin.MarketCap.AsUsdCurrency()} (Rank {coin.Rank})");
			descriptionBuilder.AppendLine($"24 hour volume: {coin.Volume.AsUsdCurrency()}");
			return descriptionBuilder.ToString();
		}

		/// <summary>
		/// Get the <paramref name="coin"/> price in USD, BTC and ETH.
		/// </summary>
		/// <param name="coin">The <see cref="ICoin"/>.</param>
		/// <returns></returns>
		public static string GetPrice(this ICoin coin)
		{
			var priceStringBuilder = new StringBuilder();
			priceStringBuilder.AppendLine(coin.PriceUsd.AsUsdCurrency(UsdPricePrecision));
			priceStringBuilder.AppendLine($"{coin.PriceBtc} BTC");
			priceStringBuilder.AppendLine($"{coin.PriceEth} ETH");
			return priceStringBuilder.ToString();
		}

		/// <summary>
		/// Get the <paramref name="coin"/> price summary in USD and BTC.
		/// </summary>
		/// <param name="coin"></param>
		/// <returns></returns>
		public static string GetPriceSummary(this ICoin coin) => $"{coin.PriceUsd.AsUsdCurrency(UsdPricePrecision)}/{coin.PriceBtc} BTC";

		/// <summary>
		/// Get the <paramref name="coin"/> title.
		/// </summary>
		/// <param name="coin">The <see cref="ICoin"/>.</param>
		/// <returns></returns>
		public static string GetTitle(this ICoin coin) => $"{coin.Name} ({coin.Symbol})";
	}
}
