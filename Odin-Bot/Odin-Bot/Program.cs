using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Victoria;
using Microsoft.Extensions.DependencyInjection;
using Odin_Bot.Handlers;
using Odin_Bot.Services;
using Discord.Commands;

namespace Odin_Bot {
    class Program {
        private DiscordSocketClient _client;
        private ServiceProvider _services;
        private Lavalink _lavaLink;

        // Start main as async ( StartAsync() )
        static void Main(string[] args) 
        => new Program().StartAsync().GetAwaiter().GetResult();

        public async Task StartAsync() {
            _services = ConfigureServices();
            _client = _services.GetRequiredService<DiscordSocketClient>();
            _lavaLink = _services.GetRequiredService<Lavalink>();

            // Client events
            _lavaLink.Log += LogAsync;
            _client.Log += LogAsync;
            _services.GetRequiredService<CommandService>().Log += LogAsync;
            _client.Ready += OnReadyAsync;

            // Quit if token is null or empty
            if (Config.bot.token == "" || Config.bot.token == null) return;

            // Set rules for connection
            _client = new DiscordSocketClient(new DiscordSocketConfig {
                LogLevel = LogSeverity.Verbose
            });

            // Log messages
            _client.Log += LogAsync;

            // Connect to discord and start commandhandler
            await _client.LoginAsync(TokenType.Bot, Config.bot.token);
            await _client.StartAsync();
            //_handler = new CommandHandler(_services);
            //await _handler.InitializeAsync(_client);

            await _services.GetRequiredService<CommandHandler>().InitializeAsync();

            await Task.Delay(-1);
        }

        /* Used when the Client Fires the ReadyEvent. */
        private async Task OnReadyAsync() {
            try {
                var node = await _lavaLink.AddNodeAsync(_client, new Configuration {
                    Severity = LogSeverity.Info
                });
                node.TrackFinished += _services.GetService<AudioService>().OnFinshed;
                await _client.SetGameAsync("Managing Aesir");
            } catch (Exception ex) {
                await LoggingService.LogInformationAsync(ex.Source, ex.Message);
            }
        }

        /*Used whenever we want to log something to the Console. 
            Todo: Hook in a Custom LoggingService. */
        private async Task LogAsync(LogMessage logMessage) {
            await LoggingService.LogAsync(logMessage.Source, logMessage.Severity, logMessage.Message);
        }

        /* Configure our Services for Dependency Injection. */
        private ServiceProvider ConfigureServices() {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<Lavalink>()
                .AddSingleton<AudioService>()
                .AddSingleton<BotService>()
                .BuildServiceProvider();
        }
    }
}
