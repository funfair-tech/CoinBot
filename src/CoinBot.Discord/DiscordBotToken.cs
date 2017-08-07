using Newtonsoft.Json;
using System.IO;

namespace CoinBot.Discord
{
    public class DiscordBotToken
    {
        public static DiscordBotToken Load(string jsonFile)
        {
            return JsonConvert.DeserializeObject<DiscordBotToken>(File.ReadAllText(jsonFile));
        }

        public string Token { get; set; }
    }
}
