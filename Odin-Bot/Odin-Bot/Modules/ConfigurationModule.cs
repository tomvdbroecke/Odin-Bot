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

namespace Odin_Bot.Modules {
    public class ConfigurationModule : ModuleBase<SocketCommandContext> {
        [Command("botchannel")]
        public async Task Botchannel() {
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
    }
}
