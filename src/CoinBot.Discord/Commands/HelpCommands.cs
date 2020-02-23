using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace CoinBot.Discord.Commands
{
    public sealed class HelpCommands : CommandBase
    {
        private readonly CommandService _commandService;

        public HelpCommands(CommandService commandService)
        {
            this._commandService = commandService;
        }

        [Command(text: "help")]
        [Summary(text: "prints this help text, which you've already figured out")]
        public Task HelpAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithTitle(title: "Help");
            AddAuthor(builder);
            AddFooter(builder);

            StringBuilder stringBuilder = new StringBuilder();

            foreach (CommandInfo command in this._commandService.Commands)
            {
                stringBuilder.AppendLine($"!{command.Name} - {command.Summary}");
            }

            builder.WithDescription(stringBuilder.ToString());

            return this.ReplyAsync(string.Empty, isTTS: false, builder.Build());
        }
    }
}