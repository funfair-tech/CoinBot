using Discord.Commands;
using System.Threading.Tasks;

namespace CoinBot.Discord.Commands
{
    public class DiagnosticCommands : ModuleBase
    {
        [Command("say"), Summary("echo a message")]
        public async Task Say([Remainder, Summary("The text to echo")] string echo)
        {
            await ReplyAsync(echo);
        }

        [Command("ping"), Summary("ping the coinbot")]
        public async Task Ping()
        {
            await ReplyAsync("pong!");
        }
    }
}
