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
using System.Diagnostics;
using System.IO;

namespace Odin_Bot {
    class Program {
        private DiscordSocketClient _client;
        private CommandService _cmdService;
        private SchedulerService _scheduler;
        private IServiceProvider _services;
        private CommandHandler _cmdHandler;

        // Start main as async ( StartAsync() )
        static void Main(string[] args) 
        => new Program().StartAsync().GetAwaiter().GetResult();

        public async Task StartAsync(DiscordSocketClient client = null, CommandService cmdService = null) {
            // Start javalink client
            //await StartLavalink();

            _client = client ?? new DiscordSocketClient(new DiscordSocketConfig {
                AlwaysDownloadUsers = true,
                MessageCacheSize = 50,
                LogLevel = LogSeverity.Debug
            });

            _cmdService = cmdService ?? new CommandService(new CommandServiceConfig {
                LogLevel = LogSeverity.Verbose,
                CaseSensitiveCommands = false
            });

            // Quit if token is null or empty
            if (Config.bot.token == "" || Config.bot.token == null) {
                await LogAsync(new LogMessage(LogSeverity.Debug, "discord", "No server token found."));
                Console.Read();
                return;
            }

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

            _services = SetupServices();
            _cmdHandler = new CommandHandler(_client, _cmdService, _services);
            await _cmdHandler.InitializeAsync();

            await _services.GetRequiredService<AudioService>().InitializeAsync();

            // Start and asign scheduler
            await StartScheduler();
            _scheduler = new SchedulerService(new XivApiService(), new CalendarService());
            _scheduler.HourlyCalendarUpdate(_client);

            await Task.Delay(-1);
        }

        private async Task StartLavalink() {
            ProcessStartInfo processInfo;
            Process lavalinkProcess;

            // Get batch path
            string fileName = "Resources/StartLavalink.bat";
            FileInfo f = new FileInfo(fileName);
            string fullname = f.FullName;

            // Get working directory path
            string directoryName = "Resources";
            DirectoryInfo d = new DirectoryInfo(directoryName);
            string directoryPath = d.FullName;

            // Log starting of lavalink
            await LogAsync(new LogMessage(LogSeverity.Info, "Lavalink", "Starting Lavalink from Batch file at: " + fullname));

            // Set process info
            processInfo = new ProcessStartInfo("cmd.exe", "/c\"" + fullname + "\"");
            processInfo.WindowStyle = ProcessWindowStyle.Normal;
            processInfo.WorkingDirectory = directoryPath;

            // Start the process and sleep for 10s
            lavalinkProcess = Process.Start(processInfo);
            await LogAsync(new LogMessage(LogSeverity.Info, "Lavalink", "Process started, waiting 10 seconds to allow Lavalink to start."));
            System.Threading.Thread.Sleep(10000);
        }

        private async Task StartScheduler() {
            // Find nearest hour
            DateTime now = DateTime.Now;
            DateTime roundedNow = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
            if (now.Minute > 0 || now.Second > 0) {
                roundedNow = roundedNow.AddHours(1);
                roundedNow = roundedNow.AddMinutes(1);
            }

            // Every minute (full minutes)
            Scheduler.IntervalInMinutes(roundedNow.Hour - 1, roundedNow.Minute, 1, async () => {
                await LogAsync(new LogMessage(LogSeverity.Info, "Scheduler", "60 Second checks fired."));

                // 60 second functions
                await _cmdHandler.ClearCommandsInLastMinute();
                await _scheduler.AnnounceEvents(_client);
                await _scheduler.SetRandomGame(_client);
            });

            // Every hour (starts from next full hour)
            Scheduler.IntervalInHours(roundedNow.Hour, 0, 1, async () => {
                await LogAsync(new LogMessage(LogSeverity.Info, "Scheduler", "Hourly scheduler fired."));

                // Hourly functions
                await _scheduler.HourlyCalendarUpdate(_client);
            });

            // Every day (starts from next day at 11:29 PM)
            Scheduler.IntervalInDays(23, 59, 1, async () => {
                await LogAsync(new LogMessage(LogSeverity.Info, "Scheduler", "Daily scheduler fired."));

                // Daily functions
                await _scheduler.DailyModeratorReport(_client);
                await _scheduler.ClearBeanRequests(_client);
            });
        }

        /*Used whenever we want to log something to the Console. */
        private async Task LogAsync(LogMessage logMessage) {
            await LoggingService.LogAsync(logMessage.Source, logMessage.Severity, logMessage.Message);
        }

        /* Configure our Services for Dependency Injection. */
        private IServiceProvider SetupServices()
            => new ServiceCollection()
            .AddSingleton(_client)
            .AddSingleton(_cmdService)
            .AddSingleton<LavaRestClient>()
            .AddSingleton<LavaSocketClient>()
            .AddSingleton<AudioService>()
            .AddSingleton<MiscService>()
            .AddSingleton<XivApiService>()
            .AddSingleton<SchedulerService>()
            .AddSingleton<CalendarService>()
            .BuildServiceProvider();
    }
}
