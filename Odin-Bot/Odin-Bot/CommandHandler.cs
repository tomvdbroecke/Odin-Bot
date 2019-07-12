using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Odin_Bot {
    class CommandHandler {
        DiscordSocketClient _client;
        CommandService _service;

        // Handle discord connection
        public async Task InitializeAsync(DiscordSocketClient client) {
            _client = client;
            _service = new CommandService();
            await _service.AddModulesAsync(Assembly.GetEntryAssembly(), null);
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
                var result = await _service.ExecuteAsync(context, argPos, null);

                // Write any errors to console
                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand) {
                    Console.WriteLine(result.ErrorReason);
                }
            }
        }
    }
}
