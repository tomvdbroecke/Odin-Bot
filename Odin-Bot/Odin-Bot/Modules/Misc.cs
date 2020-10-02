using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Flurl;
using Flurl.Http;
using System.Net.Http;
using Newtonsoft.Json;
using Odin_Bot.Services;

namespace Odin_Bot.Modules {
    public class Misc : ModuleBase<SocketCommandContext> {
        private MiscService _miscService;

        public Misc(MiscService miscService)
        {
            _miscService = miscService;
        }

        [Command("ping")]
        public async Task Ping() {
            await ReplyAsync("Pong, HEATHEN! `" + Context.Client.Latency + "ms`");
        }

        [Command("info")]
        public async Task Info() {
            var result = await _miscService.DisplayInfoAsync(Context);
            await ReplyAsync("", false, result);
        }

        [Command("todo")]
        public async Task Todo() {
            await ReplyAsync("Todo List\n" +
                "```css\n" +
                "- Automatically add events to the calendar and ping participants\n" +
                "```");
        }

        [Command("help")]
        public async Task Help([Remainder]string message = null) {
            // If no section was given
            if (message == null) {
                await ReplyAsync("What do you need help with? HEATHEN!\n" +
                    "```css\n" +
                    "Format: .help [SECTION]\n" +
                    "Sections:\n" +
                    "- misc\n" +
                    "- ffxiv\n" +
                    "- music\n" +
                    "- events\n" +
                    "- fun\n" +
                    "- moderator\n" +
                    "```");
                return;
            }
            
            // If section is misc
            if (message == "misc") {
                await ReplyAsync("**Misc Help**\n" +
                    "```css\n" +
                    Config.bot.cmdPrefix + "ping (Pings the bot and returns the latency.)\n\n" +
                    Config.bot.cmdPrefix + "info (Gives info about the bot and the current server it's connected to.)\n\n" +
                    Config.bot.cmdPrefix + "help (Gives you help with bot commands.)\n\n" +
                    Config.bot.cmdPrefix + "time (Shows you the current system time.)\n\n" +
                    Config.bot.cmdPrefix + "todo (Gives a todo list for the developer.)\n\n" +
                    "```");
                return;
            }

            // If section is ffxiv
            if (message == "ffxiv") {
                await ReplyAsync("**FFXIV Help**\n" +
                    "```css\n" +
                    Config.bot.cmdPrefix + "fcinfo (Gives information about the FC.)\n\n" +
                    Config.bot.cmdPrefix + "fcmembers (Shows all FC members and their roles.)\n\n" +
                    Config.bot.cmdPrefix + "serverstatus [DATA CENTER] (Shows FFXIV server status per data center. If no data center is given, a list of data centers will be displayed.)\n\n" +
                    "```");
                return;
            }

            // If section is music
            if (message == "music") {
                await ReplyAsync("**Music Help**\n" +
                    "```css\n" +
                    Config.bot.cmdPrefix + "join (Makes Odin join the voice channel you're currently connected to.)\n\n" +
                    Config.bot.cmdPrefix + "leave (Makes Odin leave the voice channel you're currently connected to.)\n\n" +
                    Config.bot.cmdPrefix + "play [LINK / SEARCH QUERY] (Plays the submitted audio track, or adds it to the queue if something is already playing.)\n\n" +
                    Config.bot.cmdPrefix + "stop (Stops the playback of audio, and cancels out the queue.)\n\n" +
                    Config.bot.cmdPrefix + "skip (Skips the current audio track, and moves to the next track in the queue.)\n\n" +
                    Config.bot.cmdPrefix + "volume [VOLUME (2 - 150)] (Sets the volume of the audio to the submitted volume.)\n\n" +
                    Config.bot.cmdPrefix + "pause (Pauses the current audio playback.)\n\n" +
                    Config.bot.cmdPrefix + "resume (Resumes the current audio playback.)\n\n" +
                    "```");
                return;
            }

            // If section is events
            if (message == "events") {
                await ReplyAsync("**Event Help**\n" +
                    "```css\n" +
                    Config.bot.cmdPrefix + "event [TITLE];[DESCRIPTION];[DATE+TIME];[MAX SIGNUPS (OPTIONAL)] (Creates an event in the current channel.)\n\n" +
                    Config.bot.cmdPrefix + "lightpartyevent [TITLE];[DESCRIPTION];[DATE+TIME] (Creates a light party event in the current channel. Signups are split as follows: 1 Tank, 1 Healer, 2 DPS.)\n\n" +
                    Config.bot.cmdPrefix + "fullpartyevent [TITLE];[DESCRIPTION];[DATE+TIME] (Creates a full party event in the current channel. Signups are split as follows: 2 Tanks, 2 Healers, 4 DPS.)\n\n" +
                    "```");
                return;
            }

            // If section is fun
            if (message == "fun") {
                await ReplyAsync("**Fun Help**\n" +
                    "```css\n" +
                    Config.bot.cmdPrefix + "beans (Tells you the amount of beans you've acquired.)\n\n" +
                    Config.bot.cmdPrefix + "getbeans (Asks Odin for beans, be sure not to push him too much...)\n\n" +
                    Config.bot.cmdPrefix + "beanchance (Tells you how much % chance you have to get more beans today.)\n\n" +
                    Config.bot.cmdPrefix + "beansleaderboard (Shows you the top 10 people with the most beans.)\n\n" +
                    Config.bot.cmdPrefix + "birb (Shows you a bird.)\n\n" +
                    "```");
                return;
            }

            // If section is moderator
            if (message == "events") {
                // REQUIRE MODERATOR
                if (!await PermissionService.RequireModerator(Context))
                    return;

                await ReplyAsync("**Moderator Help**\n" +
                    "```css\n" +
                    Config.bot.cmdPrefix + "displaycalendar (Displays a guild calendar.)\n\n" +
                    Config.bot.cmdPrefix + "botchannel (Toggles the Bot's permission to use the current channel.)\n\n" +
                    Config.bot.cmdPrefix + "moderatorchannel (Sets the current channel as the moderator channel.) {Owner Only}\n\n" +
                    Config.bot.cmdPrefix + "moderatorrole [MENTION ROLE] (Toggles the mentioned role as a moderator role.) {Owner Only}\n\n" +
                    "```");
                return;
            }

            await ReplyAsync(Config.pre.error + " Invalid section!\n" +
                    "```css\n" +
                    "Format: .help [SECTION]\n" +
                    "Sections:\n" +
                    "- misc\n" +
                    "- ffxiv\n" +
                    "- music\n" +
                    "- events\n" +
                    "- fun\n" +
                    "- moderator\n" +
                    "```");
            return;
        }

        [Command("avatar")]
        public async Task Avatar(string query) {
            var user = Context.Message.MentionedUsers.First();
            string[] str = user.GetAvatarUrl().Split('?');
            await ReplyAsync(str[0]);
        }

        [Command("time")]
        public async Task Time() {
            await ReplyAsync("Current system time is `" + DateTime.Now.ToString("HH:mm") + "`");
        }

    }
}
