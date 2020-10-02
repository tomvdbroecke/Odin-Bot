﻿using System;
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
using System.Text.RegularExpressions;

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

        /// Warning: JSON parsing is retarded in this function, loads of rexeg and string manipulation was required
        ///          Fuck with this function at risk of own sanity
        [Command("Serverstatus")]
        public async Task Serverstatus([Remainder] string message = "") {
            dynamic info = null;
            info = await _xivApiService.DataCenters();

            if (info == null) {
                await ReplyAsync(Config.pre.error + " Could not retrieve Data Center information.");
                return;
            }

            // If no Data Center is passed, give list of Data Centers
            if (message == "") {
                await ReplyAsync(Context.User.Mention + " Specify your data center with: `" + Config.bot.cmdPrefix + "serverstatus [Data Center]`", false, await EmbedHandler.CreateDataCentersEmbed(info));
                return;
            }

            // Return server status
            dynamic status = null;
            status = await _xivApiService.ServerStatus();

            if (status == null) {
                await ReplyAsync(Config.pre.error + " Could not retrieve Server Status information.");
                return;
            }

            List<string> servers = new List<string>();
            bool found = false;
            string dcName = "";
            foreach (var datacenter in info) {
                dcName = datacenter.ToString().Split('"')[1];
                if (dcName.ToLower() == message.ToLower()) {
                    found = true;    

                    // Don't ask
                    var arrayString = datacenter.ToString().Split('[');
                    var str = arrayString[1].Substring(0, arrayString[1].Length - 1);
                    dynamic ser = JsonConvert.DeserializeObject("[" + Regex.Replace(str, @"\s+", string.Empty) + "]");
                    foreach (var t in ser) {
                        servers.Add(t.ToString());
                    }
                }

                if (found)
                    break;
            }

            if (!found) {
                await ReplyAsync(Config.pre.error + " No Data Center by the name \"" + message + "\" was found.");
                return;
            }

            await ReplyAsync(Context.User.Mention, false, await EmbedHandler.CreateServerStatusEmbed(servers, status, dcName));
        }
    }
}
