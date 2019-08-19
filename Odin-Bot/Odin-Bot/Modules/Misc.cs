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

        [Command("help")]
        public async Task Help([Remainder]string message = null) {
            // If no section was given
            if (message == null) {
                await ReplyAsync("What do you need help with? HEATHEN!\n" +
                    "```" +
                    "Format: .help [SECTION]\n" +
                    "Sections:\n" +
                    "- misc\n" +
                    "- ffxiv\n" +
                    "- music\n" +
                    "- events\n" +
                    "```");
                return;
            }
            
            // If section is misc
            if (message == "misc") {
                await ReplyAsync("This help section is unfinished.");
                return;
            }

            // If section is ffxiv
            if (message == "ffxiv") {
                await ReplyAsync("This help section is unfinished.");
                return;
            }

            // If section is music
            if (message == "music") {
                await ReplyAsync("This help section is unfinished.");
                return;
            }

            // If section is events
            if (message == "events") {
                await ReplyAsync("This help section is unfinished.");
                return;
            }

            await ReplyAsync(Config.pre.error + "Invalid section!\n" +
                    "```" +
                    "Format: .help [SECTION]\n" +
                    "Sections:\n" +
                    "- misc\n" +
                    "- ffxiv\n" +
                    "- music\n" +
                    "- events\n" +
                    "```");
            return;
        }

    }
}
