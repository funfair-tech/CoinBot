using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CoinBot.Discord;

public sealed class DiscordBot : DiscordSocketClient
{
    /// <summary>
    ///     The General channel name.
    /// </summary>
    private const string GENERAL_CHANNEL_NAME = "general";

    /// <summary>
    ///     The <see cref="CommandService" />.
    /// </summary>
    private readonly CommandService _commands;

    /// <summary>
    ///     The <see cref="ILogger" />.
    /// </summary>
    private readonly ILogger<DiscordBot> _logger;

    /// <summary>
    ///     The <see cref="IServiceProvider" />.
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <inheritdoc />
    /// <summary>
    ///     Constructs the <see cref="DiscordBot" />.
    /// </summary>
    /// <param name="serviceProvider">The <see cref="IServiceProvider" />.</param>
    /// <param name="commandService">The <see cref="CommandService" />.</param>
    public DiscordBot(IServiceProvider serviceProvider, CommandService commandService)
    {
        this._serviceProvider = serviceProvider;
        this._logger = this._serviceProvider.GetRequiredService<ILogger<DiscordBot>>();
        this.Log += this.HandleLogAsync;

        this._commands = commandService;
        this._commands.Log += this.HandleLogAsync;
        this.MessageReceived += this.HandleCommandAsync;
    }

    /// <summary>
    ///     Handles the <paramref name="logParam" />.
    /// </summary>
    /// <param name="logParam">The <see cref="LogMessage" />.</param>
    private Task HandleLogAsync(LogMessage logParam)
    {
        return logParam.Severity switch
        {
            LogSeverity.Debug => this.LogDebugAsync(logParam),
            LogSeverity.Verbose => this.LogInformationAsync(logParam),
            LogSeverity.Info => this.LogInformationAsync(logParam),
            LogSeverity.Warning => this.LogWarningAsync(logParam),
            LogSeverity.Error => this.LogErrorAsync(logParam),
            LogSeverity.Critical => this.LogCriticalAsync(logParam),
            _ => this.LogCriticalAsync(logParam)
        };
    }

    private Task LogCriticalAsync(LogMessage logParam)
    {
        this._logger.LogCritical(logParam.Message);

        return Task.CompletedTask;
    }

    private Task LogErrorAsync(LogMessage logParam)
    {
        if (logParam.Exception != null)
        {
            this._logger.LogError(new EventId(logParam.Exception.HResult), message: logParam.Message, logParam.Exception);
        }
        else
        {
            this._logger.LogError(logParam.Message);
        }

        return Task.CompletedTask;
    }

    private Task LogWarningAsync(LogMessage logParam)
    {
        this._logger.LogWarning(logParam.Message);

        return Task.CompletedTask;
    }

    private Task LogInformationAsync(LogMessage logParam)
    {
        this._logger.LogInformation(logParam.Message);

        return Task.CompletedTask;
    }

    private Task LogDebugAsync(LogMessage logParam)
    {
        this._logger.LogDebug(logParam.Message);

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Handles the <paramref name="messageParam" />.
    /// </summary>
    /// <param name="messageParam">The <see cref="SocketMessage" />.</param>
    /// <returns></returns>
    private async Task HandleCommandAsync(SocketMessage messageParam)
    {
        if (messageParam is not SocketUserMessage message)
        {
            return;
        }

        // don't respond to messages in general, access to all other channels can be controlled with
        // permissions on discord
        if (message.Channel.Name.Equals(value: GENERAL_CHANNEL_NAME, comparisonType: StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        // Create a number to track where the prefix ends and the command begins
        int argPos = 0;

        // Determine if the message is a command, based on if it starts with '!' or a mention prefix
        if (!(message.HasCharPrefix(c: '!', argPos: ref argPos) || message.HasMentionPrefix(user: this.CurrentUser, argPos: ref argPos)))
        {
            return;
        }

        // Create a Command Context
        CommandContext context = new(this, msg: message);

        // Execute the command. (result does not indicate a return value,
        // rather an object stating if the command executed successfully)
        IResult result = await this._commands.ExecuteAsync(context: context, argPos: argPos, services: this._serviceProvider);

        if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
        {
            await context.Channel.SendMessageAsync(result.ErrorReason);
        }
    }
}