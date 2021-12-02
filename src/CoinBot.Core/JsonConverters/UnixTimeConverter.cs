using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CoinBot.Core.JsonConverters;

/// <summary>
///     A custom Json  Converterimplementation for unix timestamps.
/// </summary>
public sealed class UnixTimeConverter : JsonConverter<DateTime>
{
    /// <summary>
    ///     The <see cref="DateTime" /> to start with when adding seconds.
    /// </summary>
    /// <remarks>It is a static readonly field for performance reasons.</remarks>
    private static readonly DateTime Epoch = new(year: 1970, month: 1, day: 1, hour: 0, minute: 0, second: 0, kind: DateTimeKind.Utc);

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string? value = reader.GetString();

            long seconds = long.Parse(value ?? string.Empty);

            return Epoch.AddSeconds(seconds);
        }

        if (reader.TokenType == JsonTokenType.Number)
        {
            long seconds = reader.GetInt64();

            return Epoch.AddSeconds(seconds);
        }

        throw new JsonException($"{nameof(Decimal)} parameters must be passed as strings");
    }
}