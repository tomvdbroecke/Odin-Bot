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
using Odin_Bot.Handlers;

namespace Odin_Bot.Modules {
    public class XivApiModule : ModuleBase<SocketCommandContext> {
        private XivApiService _xivApiService;

        public XivApiModule(XivApiService xivApiService) {
            _xivApiService = xivApiService;
        }

        [Command("fcinfo")]
        public async Task Fcinfo() {
            dynamic info = null;

            // Attempt an API request for FC info
            info = await _xivApiService.FCRequest();

            // If FC info was succesfully grabbed
            Embed embed = null;
            if (info != null) {
                embed = await EmbedHandler.CreateFcInfoEmbed(info);
            }

            // If embed was null, throw an error
            if (embed == null) {
                await ReplyAsync(Config.pre.error + " Could not retrieve FFXIV API information.");
            } else {
                await ReplyAsync("Currently in service of:", false, embed);
            }
        }

        [Command("fcmembers")]
        public async Task Fcmembers() {
            dynamic info = null;

            // Attempt an API request for FC info
            info = await _xivApiService.FCRequest(true);

            // If FC info was succesfully grabbed
            Embed embed = null;
            if (info != null) {
                embed = await EmbedHandler.CreateFcMembersInfoEmbed(info);
            }

            // If embed was null, throw an error
            if (embed == null) {
                await ReplyAsync(Config.pre.error + " Could not retrieve FFXIV API information.");
            } else {
                await ReplyAsync("", false, embed);
            }
        }
    }
}
