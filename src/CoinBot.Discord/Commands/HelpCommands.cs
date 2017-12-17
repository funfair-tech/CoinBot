using Discord;
using Discord.Commands;
using System.Text;
using System.Threading.Tasks;

namespace CoinBot.Discord.Commands
{
    public class HelpCommands : CommandBase
    {
        private readonly CommandService _commandService;

        public HelpCommands(CommandService commandService)
        {
            this._commandService = commandService;
        }

        [Command("help"), Summary("prints this help text, which you've already figured out")]
        public async Task Help()
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithTitle("Help");
            AddAuthor(builder);
            AddFooter(builder);

            StringBuilder stringBuilder = new StringBuilder();
            foreach (CommandInfo command in _commandService.Commands)
            {
                stringBuilder.AppendLine($"!{command.Name} - {command.Summary}");
            }

            builder.WithDescription(stringBuilder.ToString());

            await ReplyAsync(string.Empty, false, builder);
        }
    }
}
