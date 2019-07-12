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

namespace Odin_Bot.Modules {
    public class Misc : ModuleBase<SocketCommandContext> {

        [Command("ping")]
        public async Task Ping() {
            await Context.Channel.SendMessageAsync("Pong, HEATHEN!");
        }

        [Command("fcinfo")]
        public async Task Fcinfo() {
            dynamic info = null;

            // Attempt an API request for FC info
            try {
                string requestString = "https://xivapi.com/freecompany/" + Config.bot.fcLodestoneId + "?string=" + Config.bot.xivapiServer + "&private_key=" + Config.bot.xivapiKey;
                HttpResponseMessage req = await requestString.GetAsync();

                info = JsonConvert.DeserializeObject(
                    req.Content.ReadAsStringAsync().Result
                );
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }

            // If FC info was succesfully grabbed
            if (info != null) {
                var embed = new EmbedBuilder();
                embed.WithTitle(info.FreeCompany.Name.ToString() + " <" + info.FreeCompany.Tag.ToString() + ">");
                embed.WithDescription(info.FreeCompany.Slogan.ToString());

                // If GC is Maelstrom
                if (info.FreeCompany.GrandCompany.ToString() == "Maelstrom") {
                    embed.WithColor(new Color(155, 20, 39));
                    embed.WithThumbnailUrl("https://ffxiv.gamerescape.com/w/images/thumb/0/02/The_Maelstrom_Flag.png/200px-The_Maelstrom_Flag.png");
                    embed.WithImageUrl("https://ffxiv.gamerescape.com/w/images/thumb/0/02/The_Maelstrom_Flag.png/200px-The_Maelstrom_Flag.png");
                }
                
                // If GC is Immortal Flames
                if (info.FreeCompany.GrandCompany.ToString() == "Immortal Flames") {
                    embed.WithColor(new Color(63, 62, 47));
                    embed.WithThumbnailUrl("https://ffxiv.gamerescape.com/w/images/thumb/c/ca/The_Immortal_Flames_Flag.png/200px-The_Immortal_Flames_Flag.png");
                    embed.WithImageUrl("https://ffxiv.gamerescape.com/w/images/thumb/c/ca/The_Immortal_Flames_Flag.png/200px-The_Immortal_Flames_Flag.png");
                }

                // If GC is Order of the Twin Adder
                if (info.FreeCompany.GrandCompany.ToString() == "Order of the Twin Adder") {
                    embed.WithColor(new Color(232, 181, 22));
                    embed.WithThumbnailUrl("https://ffxiv.gamerescape.com/w/images/thumb/8/8b/The_Order_of_the_Twin_Adder_Flag.png/200px-The_Order_of_the_Twin_Adder_Flag.png");
                    embed.WithImageUrl("https://ffxiv.gamerescape.com/w/images/thumb/8/8b/The_Order_of_the_Twin_Adder_Flag.png/200px-The_Order_of_the_Twin_Adder_Flag.png");
                }

                embed.AddField("Active Members", info.FreeCompany.ActiveMemberCount.ToString(), true
                ).Build();
                embed.AddField("FC Rank", info.FreeCompany.Rank.ToString(), true
                ).Build();
                embed.AddField("Weekly Ranking", info.FreeCompany.Ranking.Weekly.ToString(), true
                ).Build();
                embed.AddField("Monthly Ranking", info.FreeCompany.Ranking.Monthly.ToString(), true
                ).Build();
                embed.AddField("Server", info.FreeCompany.Server.ToString()
                ).Build();

                embed.WithCurrentTimestamp();
                embed.WithFooter("ID: " + info.FreeCompany.ID.ToString());

                await Context.Channel.SendMessageAsync("Currently in service of:", false, embed.Build());
            }
        }

        [Command("fcmembers")]
        public async Task Fcmembers() {
            dynamic info = null;

            // Attempt an API request for FC info
            try {
                string requestString = "https://xivapi.com/freecompany/" + Config.bot.fcLodestoneId + "?data=FCM&string=" + Config.bot.xivapiServer + "&private_key=" + Config.bot.xivapiKey;
                HttpResponseMessage req = await requestString.GetAsync();

                info = JsonConvert.DeserializeObject(
                    req.Content.ReadAsStringAsync().Result
                );
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }

            // If FC info was succesfully grabbed
            if (info != null) {
                var embed = new EmbedBuilder();
                embed.WithTitle(info.FreeCompany.Name.ToString() + " <" + info.FreeCompany.Tag.ToString() + ">");
                embed.WithDescription(info.FreeCompany.Slogan.ToString());

                // If GC is Maelstrom
                if (info.FreeCompany.GrandCompany.ToString() == "Maelstrom") {
                    embed.WithColor(new Color(155, 20, 39));
                    embed.WithThumbnailUrl("https://ffxiv.gamerescape.com/w/images/thumb/0/02/The_Maelstrom_Flag.png/200px-The_Maelstrom_Flag.png");
                }

                // If GC is Immortal Flames
                if (info.FreeCompany.GrandCompany.ToString() == "Immortal Flames") {
                    embed.WithColor(new Color(63, 62, 47));
                    embed.WithThumbnailUrl("https://ffxiv.gamerescape.com/w/images/thumb/c/ca/The_Immortal_Flames_Flag.png/200px-The_Immortal_Flames_Flag.png");
                }

                // If GC is Order of the Twin Adder
                if (info.FreeCompany.GrandCompany.ToString() == "Order of the Twin Adder") {
                    embed.WithColor(new Color(232, 181, 22));
                    embed.WithThumbnailUrl("https://ffxiv.gamerescape.com/w/images/thumb/8/8b/The_Order_of_the_Twin_Adder_Flag.png/200px-The_Order_of_the_Twin_Adder_Flag.png");
                }

                embed.AddField("Active Members", info.FreeCompany.ActiveMemberCount.ToString()
                ).Build();
                
                foreach (var member in info.FreeCompanyMembers) {

                    embed.AddField(member.Name.ToString(), "**Rank:** " + member.Rank.ToString(), true)
                    .Build();

                }

                embed.WithCurrentTimestamp();
                embed.WithFooter("ID: " + info.FreeCompany.ID.ToString());

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
        }

    }
}
