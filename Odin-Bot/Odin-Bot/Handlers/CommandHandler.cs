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
using Odin_Bot.Extensions;
using Discord.Rest;

namespace Odin_Bot.Handlers {
    class CommandHandler {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _cmdService;
        private readonly IServiceProvider _services;

        public Dictionary<ulong, uint> commandsInLastMinute = new Dictionary<ulong, uint>();

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

            List<ulong> newCalendarList = new List<ulong>();
            foreach (ulong id in Config.calendarIdTracker) {
                if (cache.Id != id) {
                    newCalendarList.Add(id);
                }
            }

            Config.calendarIdTracker = newCalendarList;

            var config = new Config();
            await config.SaveMessageIdTracker();
            await config.SaveCalendarIdTracker();
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
                    if (em.Footer.Value.ToString().Contains("Light Party Event")) {
                        await HandleLightPartyEventAsync(cache, channel, reaction, message, true);
                    } else if (em.Footer.Value.ToString().Contains("Full Party Event")) {
                        await HandleFullPartyEventAsync(cache, channel, reaction, message, true);
                    } else {
                        await HandleEventAsync(cache, channel, reaction, message, true);
                    }
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
                    if (em.Footer.Value.ToString().Contains("Light Party Event")) {
                        await HandleLightPartyEventAsync(cache, channel, reaction, message, false);
                    } else if (em.Footer.Value.ToString().Contains("Full Party Event")) {
                        await HandleFullPartyEventAsync(cache, channel, reaction, message, false);
                    } else {
                        await HandleEventAsync(cache, channel, reaction, message, false);
                    }
                }
            } catch (Exception e) {
                await LogAsync(new LogMessage(LogSeverity.Debug, "bot", e.ToString()));
            }
        }

        private async Task HandleLightPartyEventAsync(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction, RestUserMessage message, bool reactionAdded) {
            var chnl = message.Channel as SocketGuildChannel;
            SocketGuild guild = chnl.Guild;
            IEmote e1 = guild.Emotes.First(e => e.Name == "tank");
            IEmote e2 = guild.Emotes.First(e => e.Name == "healer");
            IEmote e3 = guild.Emotes.First(e => e.Name == "dps");

            var em = message.Embeds.First();
            var fields = em.Fields;
            List<string> t = new List<string>();
            List<string> h = new List<string>();
            List<string> d = new List<string>();

            // Iterate over fields and grab data needed
            string dateTime = "";
            for (int i = 0; i < fields.Count(); i++) {
                if (fields[i].Name == "When?") {
                    dateTime = fields[i].Value;
                }
            }

            // Refresh tank, healer and dps lists
            var tank = message.GetReactionUsersAsync(e1, 100);
            await tank.ForEachAsync(users => {
                for (int i = 0; i < users.Count(); i++) {
                    if (!users.ElementAt(i).IsBot) {
                        t.Add(users.ElementAt(i).Mention);
                    }
                }
            });
            var healer = message.GetReactionUsersAsync(e2, 100);
            await healer.ForEachAsync(users => {
                for (int i = 0; i < users.Count(); i++) {
                    if (!users.ElementAt(i).IsBot) {
                        h.Add(users.ElementAt(i).Mention);
                    }
                }
            });
            var dps = message.GetReactionUsersAsync(e3, 100);
            await dps.ForEachAsync(users => {
                for (int i = 0; i < users.Count(); i++) {
                    if (!users.ElementAt(i).IsBot) {
                        d.Add(users.ElementAt(i).Mention);
                    }
                }
            });

            // If reaction was added
            if (reactionAdded) {
                // Check if max participants have been reached
                if (t.Count > 1 && reaction.Emote.Equals(e1)) {
                    await message.RemoveReactionAsync(e1, reaction.User.Value);
                    return;
                }
                if (h.Count > 1 && reaction.Emote.Equals(e2)) {
                    await message.RemoveReactionAsync(e2, reaction.User.Value);
                    return;
                }
                if (d.Count > 2 && reaction.Emote.Equals(e3)) {
                    await message.RemoveReactionAsync(e3, reaction.User.Value);
                    return;
                }

                // Check for and handle double reactions
                if (reaction.Emote.Equals(e1)) {
                    foreach (string n in h) {
                        if (n == reaction.User.Value.Mention) {
                            await message.RemoveReactionAsync(e2, reaction.User.Value);
                            return;
                        }
                    }
                    foreach (string n in d) {
                        if (n == reaction.User.Value.Mention) {
                            await message.RemoveReactionAsync(e3, reaction.User.Value);
                            return;
                        }
                    }
                }
                if (reaction.Emote.Equals(e2)) {
                    foreach (string n in t) {
                        if (n == reaction.User.Value.Mention) {
                            await message.RemoveReactionAsync(e1, reaction.User.Value);
                            return;
                        }
                    }
                    foreach (string n in d) {
                        if (n == reaction.User.Value.Mention) {
                            await message.RemoveReactionAsync(e3, reaction.User.Value);
                            return;
                        }
                    }
                }
                if (reaction.Emote.Equals(e3)) {
                    foreach (string n in t) {
                        if (n == reaction.User.Value.Mention) {
                            await message.RemoveReactionAsync(e1, reaction.User.Value);
                            return;
                        }
                    }
                    foreach (string n in h) {
                        if (n == reaction.User.Value.Mention) {
                            await message.RemoveReactionAsync(e2, reaction.User.Value);
                            return;
                        }
                    }
                }
            }

            var embed = await EmbedHandler.UpdateLightPartyEventEmbed(em.Title, em.Description, dateTime, em.Footer.Value.ToString(), t, h, d);
            await message.ModifyAsync(q => {
                q.Embed = embed;
            });
        }

        private async Task HandleFullPartyEventAsync(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction, RestUserMessage message, bool reactionAdded) {
            var chnl = message.Channel as SocketGuildChannel;
            SocketGuild guild = chnl.Guild;
            IEmote e1 = guild.Emotes.First(e => e.Name == "tank");
            IEmote e2 = guild.Emotes.First(e => e.Name == "healer");
            IEmote e3 = guild.Emotes.First(e => e.Name == "dps");

            var em = message.Embeds.First();
            var fields = em.Fields;
            List<string> t = new List<string>();
            List<string> h = new List<string>();
            List<string> d = new List<string>();

            // Iterate over fields and grab data needed
            string dateTime = "";
            for (int i = 0; i < fields.Count(); i++) {
                if (fields[i].Name == "When?") {
                    dateTime = fields[i].Value;
                }
            }

            // Refresh tank, healer and dps lists
            var tank = message.GetReactionUsersAsync(e1, 100);
            await tank.ForEachAsync(users => {
                for (int i = 0; i < users.Count(); i++) {
                    if (!users.ElementAt(i).IsBot) {
                        t.Add(users.ElementAt(i).Mention);
                    }
                }
            });
            var healer = message.GetReactionUsersAsync(e2, 100);
            await healer.ForEachAsync(users => {
                for (int i = 0; i < users.Count(); i++) {
                    if (!users.ElementAt(i).IsBot) {
                        h.Add(users.ElementAt(i).Mention);
                    }
                }
            });
            var dps = message.GetReactionUsersAsync(e3, 100);
            await dps.ForEachAsync(users => {
                for (int i = 0; i < users.Count(); i++) {
                    if (!users.ElementAt(i).IsBot) {
                        d.Add(users.ElementAt(i).Mention);
                    }
                }
            });

            // If reaction was added
            if (reactionAdded) {
                // Check if max participants have been reached
                if (t.Count > 2 && reaction.Emote.Equals(e1)) {
                    await message.RemoveReactionAsync(e1, reaction.User.Value);
                    return;
                }
                if (h.Count > 2 && reaction.Emote.Equals(e2)) {
                    await message.RemoveReactionAsync(e2, reaction.User.Value);
                    return;
                }
                if (d.Count > 4 && reaction.Emote.Equals(e3)) {
                    await message.RemoveReactionAsync(e3, reaction.User.Value);
                    return;
                }

                // Check for and handle double reactions
                if (reaction.Emote.Equals(e1)) {
                    foreach (string n in h) {
                        if (n == reaction.User.Value.Mention) {
                            await message.RemoveReactionAsync(e2, reaction.User.Value);
                            return;
                        }
                    }
                    foreach (string n in d) {
                        if (n == reaction.User.Value.Mention) {
                            await message.RemoveReactionAsync(e3, reaction.User.Value);
                            return;
                        }
                    }
                }
                if (reaction.Emote.Equals(e2)) {
                    foreach (string n in t) {
                        if (n == reaction.User.Value.Mention) {
                            await message.RemoveReactionAsync(e1, reaction.User.Value);
                            return;
                        }
                    }
                    foreach (string n in d) {
                        if (n == reaction.User.Value.Mention) {
                            await message.RemoveReactionAsync(e3, reaction.User.Value);
                            return;
                        }
                    }
                }
                if (reaction.Emote.Equals(e3)) {
                    foreach (string n in t) {
                        if (n == reaction.User.Value.Mention) {
                            await message.RemoveReactionAsync(e1, reaction.User.Value);
                            return;
                        }
                    }
                    foreach (string n in h) {
                        if (n == reaction.User.Value.Mention) {
                            await message.RemoveReactionAsync(e2, reaction.User.Value);
                            return;
                        }
                    }
                }
            }

            var embed = await EmbedHandler.UpdateFullPartyEventEmbed(em.Title, em.Description, dateTime, em.Footer.Value.ToString(), t, h, d);
            await message.ModifyAsync(q => {
                q.Embed = embed;
            });
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
            await attending.ForEachAsync(users => {
                for (int i = 0; i < users.Count(); i++) {
                    if (!users.ElementAt(i).IsBot) {
                        att.Add(users.ElementAt(i).Mention);
                    }
                }
            });
            var notAttending = message.GetReactionUsersAsync(new Emoji("\u274C"), 100);
            await notAttending.ForEachAsync(users => {
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
                // Check channel
                bool IsOwner = await PermissionService.IsOwner((SocketGuildUser)context.User);
                bool IsModerator = await PermissionService.IsModerator((SocketGuildUser)context.User);
                bool IsValidChannel = false;
                
                // Check if current channel is valid
                if (Config.channels.botChannels != null) {
                    if (Config.channels.botChannels.Contains(context.Channel.Id)) {
                        IsValidChannel = true;
                    }
                }

                // If Is owner or moderator or Channel is valid [OR IF MESSAGE IS EXCLUDED EG BOTCHANNEL COMMAND]
                if (IsOwner || IsModerator || IsValidChannel || msg.Content == Config.bot.cmdPrefix + "botchannel") {
                    if (!commandsInLastMinute.ContainsKey(context.User.Id))
                        commandsInLastMinute.Add(context.User.Id, 0);

                    // Check for spam (6 commands per minute max)
                    if (commandsInLastMinute[context.User.Id] < 8) {

                        // Handle command if in correct channel
                        var result = await _cmdService.ExecuteAsync(context, argPos, _services, MultiMatchHandling.Best);

                        // Write any errors to console
                        if (!result.IsSuccess && result.Error != CommandError.UnknownCommand) {
                            Console.WriteLine(result.ErrorReason);
                        }

                        // Add to commandsInLastMinute
                        commandsInLastMinute[context.User.Id] += 1;

                        return;

                    } else {
                        await msg.Channel.SendMessageAsync(Config.pre.error + " Slow down HEATHEN! `(Anti-Spam, maximum 8 commands per minute.)`");
                    }
                }

                // Check if there are any valid channels (botchannels)
                if (Config.channels.botChannels.IsNullOrEmpty() && !IsOwner) {
                    await msg.Channel.SendMessageAsync(Config.pre.error + " The Bot's useable channels have not yet been configured. Please use `" + Config.bot.cmdPrefix + "botchannel` in a channel you would like to add to the Bot's useable channels.");
                }
            }
        }

        /*Used whenever we want to log something to the Console. 
            Todo: Hook in a Custom LoggingService. */
        private async Task LogAsync(LogMessage logMessage) {
            await LoggingService.LogAsync(logMessage.Source, logMessage.Severity, logMessage.Message);
        }

        public async Task ClearCommandsInLastMinute() {
            commandsInLastMinute = new Dictionary<ulong, uint>();
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
