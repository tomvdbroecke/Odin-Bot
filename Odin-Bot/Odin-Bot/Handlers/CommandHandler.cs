using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Victoria;
using Microsoft.Extensions.DependencyInjection;
using Odin_Bot.Services;

namespace Odin_Bot.Handlers {
    class CommandHandler {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _cmdService;
        private readonly IServiceProvider _services;

        public CommandHandler(DiscordSocketClient client, CommandService cmdService, IServiceProvider services) {
            _client = client;
            _cmdService = cmdService;
            _services = services;
        }

        // Handle discord connection
        public async Task InitializeAsync() {
            await _cmdService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            _cmdService.Log += LogAsync;
            _client.MessageReceived += HandleCommandAsync;
        }

        // Handle commands
        private async Task HandleCommandAsync(SocketMessage s) {
            // Get msg and context, exit if null
            var msg = s as SocketUserMessage;
            if (msg == null) return;
            var context = new SocketCommandContext(_client, msg);

            // Check if incoming msg is command
            int argPos = 0;
            if (msg.HasStringPrefix(Config.bot.cmdPrefix, ref argPos) || msg.HasMentionPrefix(_client.CurrentUser, ref argPos)) { /// if true -> iscommand
                var result = await _cmdService.ExecuteAsync(context, argPos, _services, MultiMatchHandling.Best);

                // Write any errors to console
                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand) {
                    Console.WriteLine(result.ErrorReason);
                }
            }
        }

        /*Used whenever we want to log something to the Console. 
            Todo: Hook in a Custom LoggingService. */
        private Task LogAsync(LogMessage log) {
            Console.WriteLine(log.ToString());

            return Task.CompletedTask;
        }

        /*
        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result) {
            //command is unspecified when there was a search failure (command not found); we don't care about these errors
            if (!command.IsSpecified)
                return;

            //the command was succesful, we don't care about this result, unless we want to log that a command succeeded.
            if (result.IsSuccess)
                return;

            // the command failed, let's notify the user that something happened.
            await context.Channel.SendMessageAsync($"error: {result.ToString()}");
        }
        */
    }
}
