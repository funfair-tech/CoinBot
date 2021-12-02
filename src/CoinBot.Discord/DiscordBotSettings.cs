namespace CoinBot.Discord;

public sealed class DiscordBotSettings
{
    /// <summary>
    ///     The app bot user token.
    ///     Visit <see href="https://discordapp.com/developers/applications/me">My Apps</see> to retrieve a token.
    /// </summary>
    public string Token { get; set; } = default!;
}