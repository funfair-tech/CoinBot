using Newtonsoft.Json;
using System.IO;

namespace CoinBot.Discord
{
    public class BotToken
    {
        public static BotToken Load(string jsonFile)
        {
            return JsonConvert.DeserializeObject<BotToken>(File.ReadAllText(jsonFile));
        }

        public string Token { get; set; }
    }
}
