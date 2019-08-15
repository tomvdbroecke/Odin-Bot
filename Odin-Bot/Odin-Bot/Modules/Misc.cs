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
            await Context.Channel.SendMessageAsync("Pong, HEATHEN! `" + Context.Client.Latency + "ms`");
        }

        [Command("info")]
        public async Task Info() {
            await _miscService.DisplayInfoAsync(Context);
        }

    }
}
