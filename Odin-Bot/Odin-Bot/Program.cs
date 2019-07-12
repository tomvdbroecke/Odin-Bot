using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace Odin_Bot {
    class Program {
        DiscordSocketClient _client;
        CommandHandler _handler;

        // Start main as async ( StartAsync() )
        static void Main(string[] args) 
        => new Program().StartAsync().GetAwaiter().GetResult();

        public async Task StartAsync() {
            // Quit if token is null or empty
            if (Config.bot.token == "" || Config.bot.token == null) return;

            // Set rules for connection
            _client = new DiscordSocketClient(new DiscordSocketConfig {
                LogLevel = LogSeverity.Verbose
            });

            // Log messages
            _client.Log += Log;

            // Connect to discord and start commandhandler
            await _client.LoginAsync(TokenType.Bot, Config.bot.token);
            await _client.StartAsync();
            _handler = new CommandHandler();
            await _handler.InitializeAsync(_client);
            await Task.Delay(-1);
        }

        private async Task Log(LogMessage msg) {
            Console.WriteLine(msg.Message);
        }
    }
}
