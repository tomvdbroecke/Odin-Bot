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
using Discord.Rest;

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
            _client.ReactionAdded += OnReactionAdded;
            _client.ReactionRemoved += OnReactionRemoved;
            _client.MessageDeleted += OnMessageDeleted;

            await _client.SetGameAsync(Config.bot.cmdPrefix + "help");
        }

        private async Task OnMessageDeleted(Cacheable<IMessage, ulong> cache, ISocketMessageChannel channel) {
            // Check if deleted message was a tracked message
            // If so, remove from tracker and save
            List<ulong> newTrackerList = new List<ulong>();
            foreach (ulong id in Config.messageIdTracker) {
                if (cache.Id != id) {
                    newTrackerList.Add(id);
                }
            }

            Config.messageIdTracker = newTrackerList;

            var config = new Config();
            await config.SaveMessageIdTracker();
        }

        private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction) {
            // Check if reaction on message was a reaction on a tracked message
            ulong eventTrackingId = 0;
            foreach (ulong id in Config.messageIdTracker) {
                if (reaction.MessageId == id) {
                    eventTrackingId = id;
                }
            }

            // Ignore bot reactions
            if (reaction.User.Value.IsBot) {
                return;
            }

            // Execute correct handler
            try {
                var message = channel.GetMessageAsync(eventTrackingId).Result as RestUserMessage;
                var em = message.Embeds.First();

                // If tracked message is an event
                if (em.Footer.Value.ToString().Contains("Event")) {
                    await HandleEventAsync(cache, channel, reaction, message, true);
                }
            } catch (Exception e) {
                await LogAsync(new LogMessage(LogSeverity.Debug, "bot", e.ToString()));
            }
        }
        private async Task OnReactionRemoved(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction) {
            // Check if reaction on message was a reaction on a tracked message
            ulong eventTrackingId = 0;
            foreach (ulong id in Config.messageIdTracker) {
                if (reaction.MessageId == id) {
                    eventTrackingId = id;
                }
            }

            // Ignore bot reactions
            if (reaction.User.Value.IsBot) {
                return;
            }

            // Execute correct handler
            try {
                var message = channel.GetMessageAsync(eventTrackingId).Result as RestUserMessage;
                var em = message.Embeds.First();

                // If tracked message is an event
                if (em.Footer.Value.ToString().Contains("Event")) {
                    await HandleEventAsync(cache, channel, reaction, message, false);
                }
            } catch (Exception e) {
                await LogAsync(new LogMessage(LogSeverity.Debug, "bot", e.ToString()));
            }
        }

        private async Task HandleEventAsync(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction, RestUserMessage message, bool reactionAdded) {
            var em = message.Embeds.First();
            var fields = em.Fields;
            List<string> att = new List<string>();
            List<string> nAtt = new List<string>();

            // Iterate over fields and grab data needed
            int maxParticipants = 0;
            string dateTime = "";
            for (int i = 0; i < fields.Count(); i++) {
                if (fields[i].Name == "When?") {
                    dateTime = fields[i].Value;
                }
                if (fields[i].Name == "Max Participants") {
                    maxParticipants = Int32.Parse(fields[i].Value);
                }
            }

            // Refresh both 'Attending' and 'Not attending' lists
            var attending = message.GetReactionUsersAsync(new Emoji("\u2705"), 100);
            attending.ForEach(users => {
                for (int i = 0; i < users.Count(); i++) {
                    if (!users.ElementAt(i).IsBot) {
                        att.Add(users.ElementAt(i).Mention);
                    }
                }
            });
            var notAttending = message.GetReactionUsersAsync(new Emoji("\u274C"), 100);
            notAttending.ForEach(users => {
                for (int i = 0; i < users.Count(); i++) {
                    if (!users.ElementAt(i).IsBot) {
                        nAtt.Add(users.ElementAt(i).Mention);
                    }
                }
            });

            // If reaction was added
            if (reactionAdded) {
                // Check if max participants have been reached
                if (att.Count > maxParticipants && maxParticipants != 0) {
                    await message.RemoveReactionAsync(new Emoji("\u2705"), reaction.User.Value);
                    return;
                }

                // Check for and handle double reactions
                if (reaction.Emote.Equals(new Emoji("\u2705"))) {
                    foreach (string n in nAtt) {
                        if (n == reaction.User.Value.Mention) {
                            await message.RemoveReactionAsync(new Emoji("\u274C"), reaction.User.Value);
                            return;
                        }
                    }
                }
                if (reaction.Emote.Equals(new Emoji("\u274C"))) {
                    foreach (string n in att) {
                        if (n == reaction.User.Value.Mention) {
                            await message.RemoveReactionAsync(new Emoji("\u2705"), reaction.User.Value);
                            return;
                        }
                    }
                }
            }

            var embed = await EmbedHandler.UpdateEventEmbed(em.Title, em.Description, maxParticipants, dateTime, em.Footer.Value.ToString(), att, nAtt);
            await message.ModifyAsync(q => {
                q.Embed = embed;
            });
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
        private async Task LogAsync(LogMessage logMessage) {
            await LoggingService.LogAsync(logMessage.Source, logMessage.Severity, logMessage.Message);
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
