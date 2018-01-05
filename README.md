# CoinBot
Discord Cryptocurrency Bot

# Running a development version

1. Create a private Discord server by following the steps explained here: [https://support.discordapp.com/hc/en-us/articles/204849977](https://support.discordapp.com/hc/en-us/articles/204849977).

2. Create a Discord Bot and invite it to your server by following the steps explained here: [https://github.com/reactiflux/discord-irc/wiki/Creating-a-discord-bot-&-getting-a-token](https://github.com/reactiflux/discord-irc/wiki/Creating-a-discord-bot-&-getting-a-token).

3. Configure the Discord Bot User Token. You can choose one of the following configuration options:
  * Create a file named `appsettings.json` inside the `CoinBot` project and add the following contents: 
 ```json
{
  "DiscordBot": {
    "Token": "[Discord Bot User Token goes here]"
  }
}
```
  * Or add the Discord Bot User Token as an environment variable (`DiscordBot:Token`) as explained here: [https://docs.microsoft.com/en-us/aspnet/core/fundamentals/environments](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/environments)

4. Build and run the CoinBot application, you should see the CoinBot come online in Discord. It will not reply to you in the General channel!
