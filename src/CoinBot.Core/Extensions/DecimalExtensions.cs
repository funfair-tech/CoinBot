namespace CoinBot.Core.Extensions
{
	/// <summary>
	/// <see cref="decimal"/> extension methods.
	/// </summary>
	public static class DecimalExtensions
	{
		/// <summary>
		/// The default response when formatting an input parameter that is null.
		/// </summary>
		private const string UnknownResponse = "unknown";

		/// <summary>
		/// Formats the <paramref name="d"/> as a currency.
		/// </summary>
		/// <param name="d">The value to format.</param>
		/// <returns></returns>
		public static string AsPrice(this decimal? d) => d?.ToString("#,##0.#################") ?? UnknownResponse;

		/// <summary>
		/// todo
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		public static string AsVolume(this decimal? d) => d?.ToString("N2") ?? UnknownResponse;
	}
}