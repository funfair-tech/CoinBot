using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace CoinBot.Discord.Commands;

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
        EmbedBuilder builder = new();
        builder.WithTitle(title: "Help");
        AddAuthor(builder);
        AddFooter(builder);

        builder.WithDescription(this.BuildCommandDescription());

        return this.ReplyAsync(message: string.Empty, isTTS: false, builder.Build());
    }

    private string BuildCommandDescription()
    {
        static StringBuilder AppendCommand(StringBuilder stringBuilder, CommandInfo command)
        {
            return stringBuilder.Append('!')
                                .Append(command.Name)
                                .Append(" - ")
                                .AppendLine(command.Summary);
        }

        return this._commandService.Commands.Aggregate(new StringBuilder(), func: AppendCommand)
                   .ToString();
    }
}