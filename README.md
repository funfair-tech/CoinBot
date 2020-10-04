# CoinBot
Discord Cryptocurrency Bot

## Running a development version

1. Create a private Discord server by following the steps explained here: [https://support.discordapp.com/hc/en-us/articles/204849977](https://support.discordapp.com/hc/en-us/articles/204849977).

2. Create a Discord Bot and invite it to your server by following the steps explained here: [https://github.com/reactiflux/discord-irc/wiki/Creating-a-discord-bot-&-getting-a-token](https://github.com/reactiflux/discord-irc/wiki/Creating-a-discord-bot-&-getting-a-token).

3. Application configuration settings:
  * `DiscordBot:Token` [Required] Must be configured with the Discord Bot User Token.
  * `MarketManager:RefreshInterval` [Optional] The exchange refresh interval in minutes. Default is `2` minutes.


4. Choose how you want to configure the application:
  * Create a file named `appsettings.json` inside the `CoinBot` project and add the following contents: 
 ```json0
{
  "DiscordBot": {
    "Token": "[Discord Bot User Token goes here]"
  },
  "MarketManager": {
    "RefreshInterval": 2
  }
}
```
  * Or use environment variables as explained here: [https://docs.microsoft.com/en-us/aspnet/core/fundamentals/environments](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/environments). You will need to configure these variables: `DiscordBot:Token` (required) and `MarketManager:RefreshInterval` (optional).

5. Build and run the CoinBot application. You should see the CoinBot come online in Discord. It will not reply to you in the General channel!

## Changelog

View [changelog](CHANGELOG.md)

[CHANGELOG]: ./CHANGELOG.md
