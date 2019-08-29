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
using Discord.WebSocket;
using System.Text.RegularExpressions;

namespace Odin_Bot.Modules {
    public class ConfigurationModule : ModuleBase<SocketCommandContext> {
        [Command("botchannel")]
        public async Task Botchannel() {
            // REQUIRE MODERATOR
            if (!await PermissionService.RequireModerator(Context))
                return;

            // Turn array into list
            List<ulong> chList = new List<ulong>();
            if (Config.channels.botChannels != null) {
                chList = Config.channels.botChannels.ToList();
            }

            // Toggle current channel in list
            if (!chList.Contains(Context.Channel.Id)) {
                chList.Add(Context.Channel.Id);
                await Context.Channel.SendMessageAsync(Config.pre.success + " The channel <#" + Context.Channel.Id  + "> has been added to the Bot's useable channels.");
            } else {
                chList.Remove(Context.Channel.Id);
                await Context.Channel.SendMessageAsync(Config.pre.success + " The channel <#" + Context.Channel.Id + "> has been removed from the Bot's useable channels.");
            }
            // Turn list back into array
            Config.channels.botChannels = chList.ToArray();

            // Save config
            var config = new Config();
            await config.SaveChannelsConfig();
        }

        [Command("moderatorrole")]
        public async Task Moderatorrole(string param) {
            // REQUIRE OWNER
            if (!await PermissionService.RequireOwner(Context))
                return;

            // Cut id out of input
            string idStr = Regex.Replace(param, @"(\s+|@|&|'|\(|\)|<|>|#)", "");
            ulong id = 0;
            try {
                id = Convert.ToUInt64(idStr);
            } catch (Exception e) {
                await ReplyAsync(Config.pre.error + " An unknown error has occured.");
                return;
            }
            // Check if role with ID exists
            SocketRole role = null;
            role = Context.Guild.GetRole(Convert.ToUInt64(id));

            // Exit if role is null
            if (role == null) {
                await ReplyAsync(Config.pre.error + " Unknown role given.");
                return;
            }

            // Turn array into list
            List<ulong> mList = new List<ulong>();
            if (Config.roles.moderators != null) {
                mList = Config.roles.moderators.ToList();
            }

            // Toggle current role in list
            if (!mList.Contains(id)) {
                mList.Add(id);
                await Context.Channel.SendMessageAsync(Config.pre.success + " The <@&" + idStr + "> role has been added to the moderator list.");
            } else {
                mList.Remove(id);
                await Context.Channel.SendMessageAsync(Config.pre.success + " The <@&" + idStr + "> role has been removed from the moderator list.");
            }

            // Turn list back into array
            Config.roles.moderators = mList.ToArray();

            // Save config
            var config = new Config();
            await config.SaveRolesConfig();
        }

        [Command("moderatorchannel")]
        public async Task Moderatorchannel() {
            // REQUIRE OWNER
            if (!await PermissionService.RequireOwner(Context))
                return;

            // Set new moderatorchannel
            Config.channels.moderatorChannel = Context.Channel.Id;
            await Context.Channel.SendMessageAsync(Config.pre.success + " The channel <#" + Context.Channel.Id + "> is now the moderator channel.");

            // Save config
            var config = new Config();
            await config.SaveChannelsConfig();
        }
    }
}
