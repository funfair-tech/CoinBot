using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CoinBot.Discord
{
	public class DiscordBot : DiscordSocketClient
	{
		/// <summary>
		/// The General channel name.
		/// </summary>
		private const string GeneralChannelName = "general";

		/// <summary>
		/// The <see cref="IServiceProvider"/>.
		/// </summary>
		private readonly IServiceProvider _serviceProvider;

		/// <summary>
		/// The <see cref="ILogger"/>.
		/// </summary>
		private readonly ILogger _logger;

		/// <summary>
		/// The <see cref="CommandService"/>.
		/// </summary>
		private readonly CommandService _commands;

		/// <inheritdoc />
		/// <summary>
		/// Constructs the <see cref="DiscordBot" />.
		/// </summary>
		/// <param name="serviceProvider">The <see cref="IServiceProvider"/>.</param>
		/// <param name="commandService">The <see cref="CommandService"/>.</param>
		public DiscordBot(IServiceProvider serviceProvider, CommandService commandService)
		{
			_serviceProvider = serviceProvider;
			_logger = _serviceProvider.GetRequiredService<ILogger>();
			Log += HandleLog;

			_commands = commandService;
			_commands.Log += HandleLog;
			MessageReceived += HandleCommand;
		}

		/// <summary>
		/// Handles the <paramref name="logParam"/>.
		/// </summary>
		/// <param name="logParam">The <see cref="LogMessage"/>.</param>
		/// <returns></returns>
		private async Task HandleLog(LogMessage logParam)
		{
			switch (logParam.Severity)
			{
				case LogSeverity.Debug:
				{
					_logger.LogDebug(logParam.Message);
					break;
				}

				case LogSeverity.Verbose:
				{
					_logger.LogInformation(logParam.Message);
					break;
				}

				case LogSeverity.Info:
				{
					_logger.LogInformation(logParam.Message);
					break;
				}

				case LogSeverity.Warning:
				{
					_logger.LogWarning(logParam.Message);
					break;
				}

				case LogSeverity.Error:
				{
					if (logParam.Exception != null)
					{
						_logger.LogError(new EventId(logParam.Exception.HResult), logParam.Message, logParam.Exception);
					}
					else
					{
						_logger.LogError(logParam.Message);
					}
					break;
				}

				case LogSeverity.Critical:
				{
					_logger.LogCritical(logParam.Message);
					break;
				}
			}

			await Task.CompletedTask;
		}

		/// <summary>
		/// Handles the <paramref name="messageParam"/>.
		/// </summary>
		/// <param name="messageParam">The <see cref="SocketMessage"/>.</param>
		/// <returns></returns>
		private async Task HandleCommand(SocketMessage messageParam)
		{
			if (!(messageParam is SocketUserMessage message))
				return;

			// don't respond to messages in general, access to all other channels can be controlled with
			// permissions on discord
			if (message.Channel.Name.Equals(GeneralChannelName, StringComparison.OrdinalIgnoreCase))
				return;

			// Create a number to track where the prefix ends and the command begins
			var argPos = 0;

			// Determine if the message is a command, based on if it starts with '!' or a mention prefix
			if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(CurrentUser, ref argPos)))
				return;

			// Create a Command Context
			var context = new CommandContext(this, message);

			// Execute the command. (result does not indicate a return value, 
			// rather an object stating if the command executed successfully)
			var result = await _commands.ExecuteAsync(context, argPos, _serviceProvider);
			if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
				await context.Channel.SendMessageAsync(result.ErrorReason);
		}
	}
}
