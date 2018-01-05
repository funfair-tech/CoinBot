using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CoinBot.CoinSources.CoinMarketCap
{
	/// <summary>
	/// A custom <see cref="DateTimeConverterBase"/> implementation for unix timestamps.
	/// </summary>
	public class CoinMarketCapEpochConverter : DateTimeConverterBase
	{
		/// <summary>
		/// The <see cref="DateTime"/> to start with when adding seconds.
		/// </summary>
		/// <remarks>It is a static readonly field for performance reasons.</remarks>
		private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		/// <inheritdoc />
		/// <summary>
		/// TODO: Implementation is not yet needed because we do not write JSON.
		/// </summary>
		/// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" />.</param>
		/// <param name="value">The value <see cref="T:System.Object" />.</param>
		/// <param name="serializer">The <see cref="T:Newtonsoft.Json.JsonSerializer" />.</param>
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
			throw new NotImplementedException();

		/// <inheritdoc />
		/// <summary>
		/// Converts a unix timestamp (expressed as string or long) to a <see cref="DateTime"/>.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="objectType"></param>
		/// <param name="existingValue"></param>
		/// <param name="serializer"></param>
		/// <returns></returns>
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.Value == null)
				return null;

			var value = reader.Value;

			long seconds = 0;

			// The CoinMarketCap global API expresses its 'last_updated' field as an integer, while the ticker API expresses them as a string.
			switch (value)
			{
				case string _:
					seconds = long.Parse((string) reader.Value);
					break;
				case long _:
					seconds = (long) reader.Value;
					break;
			}

			return Epoch.AddSeconds(seconds);
		}
	}
}
