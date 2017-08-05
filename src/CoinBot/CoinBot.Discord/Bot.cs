using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace CoinBot.Discord
{
    public class Bot
    {
        private readonly BotToken _botToken;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        public Bot(BotToken botToken, IServiceProvider serviceProvider, ILogger logger)
        {
            this._botToken = botToken;
            this._client = new DiscordSocketClient();
            this._commands = new CommandService(new CommandServiceConfig()
            {
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Debug
            });

            this._logger = logger;
            this._serviceProvider = serviceProvider;

            this._client.MessageReceived += this.HandleCommand;

            this._client.Log += this.Log;
            this._commands.Log += this.Log;
        }

        private async Task Log(LogMessage arg)
        {
            switch (arg.Severity)
            {
                case LogSeverity.Debug:
                    {
                        this._logger.LogDebug(arg.Message);
                        break;
                    }

                case LogSeverity.Verbose:
                    {
                        this._logger.LogInformation(arg.Message);
                        break;
                    }

                case LogSeverity.Info:
                    {
                        this._logger.LogInformation(arg.Message);
                        break;
                    }

                case LogSeverity.Warning:
                    {
                        this._logger.LogWarning(arg.Message);
                        break;
                    }

                case LogSeverity.Error:
                    {
                        if (arg.Exception != null)
                        {
                            this._logger.LogError(new EventId(arg.Exception.HResult), arg.Message, arg.Exception);
                        }
                        else
                        {
                            this._logger.LogError(arg.Message);
                        }
                        break;
                    }

                case LogSeverity.Critical:
                    {
                        this._logger.LogCritical(arg.Message);
                        break;
                    }
            }

            await Task.CompletedTask;
        }

        public async Task Start()
        {
            // Discover all of the commands in this assembly and load them.
            await this._commands.AddModulesAsync(typeof(Bot).GetTypeInfo().Assembly);

            // login
            await this._client.LoginAsync(TokenType.Bot, this._botToken.Token);

            // and start
            await this._client.StartAsync();
        }

        public async Task Stop()
        {
            // and logout
            await this._client.LogoutAsync();
        }

        public async Task HandleCommand(SocketMessage messageParam)
        {
            SocketUserMessage message = messageParam as SocketUserMessage;
            if (message == null) return;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message is a command, based on if it starts with '!' or a mention prefix
            if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(this._client.CurrentUser, ref argPos))) return;

            // Create a Command Context
            CommandContext context = new CommandContext(this._client, message);

            // Execute the command. (result does not indicate a return value, 
            // rather an object stating if the command executed successfully)
            var result = await this._commands.ExecuteAsync(context, argPos, this._serviceProvider);
            if (!result.IsSuccess)
            {
                await context.Channel.SendMessageAsync(result.ErrorReason);
            }
        }
    }
}
