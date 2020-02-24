using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CoinBot.Core.JsonConverters
{
    public sealed class DecimalAsStringConverter : JsonConverter<decimal>
    {
        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException($"{typeof(decimal).Name} parameters must be passed as strings");
            }

            string s = reader.GetString();

            if (!decimal.TryParse(s, out decimal converted))
            {
                throw new JsonException($"Can't convert {s} to {typeof(decimal).Name}");
            }

            return converted;
        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
        }
    }
}