using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Odin_Bot;
using Discord.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Odin_Bot.Handlers {
    public static class EmbedHandler {
        /* This file is where we can store all the Embed Helper Tasks (So to speak). 
             We wrap all the creations of new EmbedBuilder's in a Task.Run to allow us to stick with Async calls. 
             All the Tasks here are also static which means we can call them from anywhere in our program. */
        public static async Task<Embed> CreateBasicEmbed(string title, string description, Color color) {
            var embed = await Task.Run(() => (new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(description)
                .WithColor(color)
                .WithCurrentTimestamp().Build()));
            return embed;
        }

        public static async Task<Embed> CreateErrorEmbed(string source, string error) {
            var embed = await Task.Run(() => new EmbedBuilder()
                .WithTitle($"ERROR OCCURED FROM - {source}")
                .WithDescription($"**Error Details**: \n{error}")
                .WithColor(Color.DarkRed)
                .WithCurrentTimestamp().Build());
            return embed;
        }

#region Normal Events
        public static async Task<Embed> CreateEventEmbed(string title, string description, int maxParticipants, string dateTime, string user) {
            if (maxParticipants == 0) {
                var embed = await Task.Run(() => (new EmbedBuilder()
                    .WithTitle(Utilities.UppercaseFirst(title))
                    .WithDescription(Utilities.UppercaseFirst(description))
                    .WithColor(Color.Green)
                    .WithFields(
                        new EmbedFieldBuilder()
                            .WithName("When?")
                            .WithValue(dateTime)
                            .WithIsInline(false)
                        )
                    .WithFields(
                        new EmbedFieldBuilder()
                            .WithName("Attending")
                            .WithValue("- None")
                            .WithIsInline(true)
                        )
                    .WithFields(
                        new EmbedFieldBuilder()
                            .WithName("Not Attending")
                            .WithValue("- None")
                            .WithIsInline(true)
                        )
                    .WithThumbnailUrl("https://vignette.wikia.nocookie.net/finalfantasy/images/5/5d/FFXIV_A_Realm_Restored_trophy_icon.png")
                    .WithFooter("Event created by " + user)
                    .WithCurrentTimestamp().Build()));
                return embed;
            } else {
                var embed = await Task.Run(() => (new EmbedBuilder()
                    .WithTitle(Utilities.UppercaseFirst(title))
                    .WithDescription(Utilities.UppercaseFirst(description))
                    .WithColor(Color.Green)
                    .WithFields(
                        new EmbedFieldBuilder()
                            .WithName("When?")
                            .WithValue(dateTime)
                            .WithIsInline(true)
                        )
                    .WithFields(
                        new EmbedFieldBuilder()
                            .WithName("Max Participants")
                            .WithValue(maxParticipants.ToString())
                            .WithIsInline(false)
                        )
                    .WithFields(
                        new EmbedFieldBuilder()
                            .WithName("Attending (0/" + maxParticipants.ToString() + ")")
                            .WithValue("- None")
                            .WithIsInline(true)
                        )
                    .WithFields(
                        new EmbedFieldBuilder()
                            .WithName("Not Attending")
                            .WithValue("- None")
                            .WithIsInline(true)
                        )
                    .WithThumbnailUrl("https://vignette.wikia.nocookie.net/finalfantasy/images/5/5d/FFXIV_A_Realm_Restored_trophy_icon.png")
                    .WithFooter("Event created by " + user)
                    .WithCurrentTimestamp().Build()));
                return embed;
            }
        }

        public static async Task<Embed> UpdateEventEmbed(string title, string description, int maxParticipants, string dateTime, string footer, List<string> attending, List<string> notAttending) {
            string att = "";
            for (int i = 0; i < attending.Count; i++) {
                if (i == attending.Count -1) {
                    att += attending[i];
                } else {
                    att += attending[i] + "\n";
                }
            }

            string nAtt = "";
            for (int i = 0; i < notAttending.Count; i++) {
                if (i == notAttending.Count - 1) {
                    nAtt += notAttending[i];
                } else {
                    nAtt += notAttending[i] + "\n";
                }
            }

            if (att == "") {
                att = "- None";
            }
            if (nAtt == "") {
                nAtt = "- None";
            }

            if (maxParticipants == 0) {
                var embed = await Task.Run(() => (new EmbedBuilder()
                    .WithTitle(Utilities.UppercaseFirst(title))
                    .WithDescription(Utilities.UppercaseFirst(description))
                    .WithColor(Color.Green)
                    .WithFields(
                        new EmbedFieldBuilder()
                            .WithName("When?")
                            .WithValue(dateTime)
                            .WithIsInline(false)
                        )
                    .WithFields(
                        new EmbedFieldBuilder()
                            .WithName("Attending")
                            .WithValue(att)
                            .WithIsInline(true)
                        )
                    .WithFields(
                        new EmbedFieldBuilder()
                            .WithName("Not Attending")
                            .WithValue(nAtt)
                            .WithIsInline(true)
                        )
                    .WithThumbnailUrl("https://vignette.wikia.nocookie.net/finalfantasy/images/5/5d/FFXIV_A_Realm_Restored_trophy_icon.png")
                    .WithFooter(footer)
                    .WithCurrentTimestamp().Build()));
                return embed;
            } else {
                var embed = await Task.Run(() => (new EmbedBuilder()
                    .WithTitle(Utilities.UppercaseFirst(title))
                    .WithDescription(Utilities.UppercaseFirst(description))
                    .WithColor(Color.Green)
                    .WithFields(
                        new EmbedFieldBuilder()
                            .WithName("When?")
                            .WithValue(dateTime)
                            .WithIsInline(true)
                        )
                    .WithFields(
                        new EmbedFieldBuilder()
                            .WithName("Max Participants")
                            .WithValue(maxParticipants.ToString())
                            .WithIsInline(false)
                        )
                    .WithFields(
                        new EmbedFieldBuilder()
                            .WithName("Attending (" + attending.Count + "/" + maxParticipants.ToString() + ")")
                            .WithValue(att)
                            .WithIsInline(true)
                        )
                    .WithFields(
                        new EmbedFieldBuilder()
                            .WithName("Not Attending")
                            .WithValue(nAtt)
                            .WithIsInline(true)
                        )
                    .WithThumbnailUrl("https://vignette.wikia.nocookie.net/finalfantasy/images/5/5d/FFXIV_A_Realm_Restored_trophy_icon.png")
                    .WithFooter(footer)
                    .WithCurrentTimestamp().Build()));
                return embed;
            }
        }

        #endregion

#region Light Party Events

        public static async Task<Embed> CreateLightPartyEventEmbed(string title, string description, string dateTime, string user) {
            var embed = await Task.Run(() => (new EmbedBuilder()
                .WithTitle(Utilities.UppercaseFirst(title))
                .WithDescription(Utilities.UppercaseFirst(description))
                .WithColor(Color.Green)
                .WithFields(
                    new EmbedFieldBuilder()
                        .WithName("When?")
                        .WithValue(dateTime)
                        .WithIsInline(false)
                    )
                .WithFields(
                    new EmbedFieldBuilder()
                        .WithName("Tank")
                        .WithValue("- None")
                        .WithIsInline(true)
                    )
                .WithFields(
                    new EmbedFieldBuilder()
                        .WithName("Healer")
                        .WithValue("- None")
                        .WithIsInline(true)
                    )
                .WithFields(
                    new EmbedFieldBuilder()
                        .WithName("DPS")
                        .WithValue("- None")
                        .WithIsInline(true)
                    )
                .WithThumbnailUrl("https://ffxiv.consolegameswiki.com/mediawiki/images/0/05/Quest_icon.png")
                .WithFooter("Light Party Event created by " + user)
                .WithCurrentTimestamp().Build()));
            return embed;
        }

        public static async Task<Embed> UpdateLightPartyEventEmbed(string title, string description, string dateTime, string footer, List<string> tanks, List<string> healers, List<string> dps) {
            string t = "";
            for (int i = 0; i < tanks.Count; i++) {
                if (i == tanks.Count - 1) {
                    t += tanks[i];
                } else {
                    t += tanks[i] + "\n";
                }
            }

            string h = "";
            for (int i = 0; i < healers.Count; i++) {
                if (i == healers.Count - 1) {
                    h += healers[i];
                } else {
                    h += healers[i] + "\n";
                }
            }

            string d = "";
            for (int i = 0; i < dps.Count; i++) {
                if (i == dps.Count - 1) {
                    d += dps[i];
                } else {
                    d += dps[i] + "\n";
                }
            }

            if (t == "") {
                t = "- None";
            }
            if (h == "") {
                h = "- None";
            }
            if (d == "") {
                d = "- None";
            }

            var embed = await Task.Run(() => (new EmbedBuilder()
                .WithTitle(Utilities.UppercaseFirst(title))
                .WithDescription(Utilities.UppercaseFirst(description))
                .WithColor(Color.Green)
                .WithFields(
                    new EmbedFieldBuilder()
                        .WithName("When?")
                        .WithValue(dateTime)
                        .WithIsInline(false)
                    )
                .WithFields(
                    new EmbedFieldBuilder()
                        .WithName("Tank")
                        .WithValue(t)
                        .WithIsInline(true)
                    )
                .WithFields(
                    new EmbedFieldBuilder()
                        .WithName("Healer")
                        .WithValue(h)
                        .WithIsInline(true)
                    )
                .WithFields(
                    new EmbedFieldBuilder()
                        .WithName("DPS")
                        .WithValue(d)
                        .WithIsInline(true)
                    )
                .WithThumbnailUrl("https://ffxiv.consolegameswiki.com/mediawiki/images/0/05/Quest_icon.png")
                .WithFooter(footer)
                .WithCurrentTimestamp().Build()));
            return embed;
        }

        #endregion

#region Full Party Events

        public static async Task<Embed> CreateFullPartyEventEmbed(string title, string description, string dateTime, string user) {
            var embed = await Task.Run(() => (new EmbedBuilder()
                .WithTitle(Utilities.UppercaseFirst(title))
                .WithDescription(Utilities.UppercaseFirst(description))
                .WithColor(Color.Green)
                .WithFields(
                    new EmbedFieldBuilder()
                        .WithName("When?")
                        .WithValue(dateTime)
                        .WithIsInline(false)
                    )
                .WithFields(
                    new EmbedFieldBuilder()
                        .WithName("Tanks")
                        .WithValue("- None")
                        .WithIsInline(true)
                    )
                .WithFields(
                    new EmbedFieldBuilder()
                        .WithName("Healers")
                        .WithValue("- None")
                        .WithIsInline(true)
                    )
                .WithFields(
                    new EmbedFieldBuilder()
                        .WithName("DPS")
                        .WithValue("- None")
                        .WithIsInline(true)
                    )
                .WithThumbnailUrl("https://ffxiv.consolegameswiki.com/mediawiki/images/8/87/Main_Scenario_Quest_icon.png")
                .WithFooter("Full Party Event created by " + user)
                .WithCurrentTimestamp().Build()));
            return embed;
        }

        public static async Task<Embed> UpdateFullPartyEventEmbed(string title, string description, string dateTime, string footer, List<string> tanks, List<string> healers, List<string> dps) {
            string t = "";
            for (int i = 0; i < tanks.Count; i++) {
                if (i == tanks.Count - 1) {
                    t += tanks[i];
                } else {
                    t += tanks[i] + "\n";
                }
            }

            string h = "";
            for (int i = 0; i < healers.Count; i++) {
                if (i == healers.Count - 1) {
                    h += healers[i];
                } else {
                    h += healers[i] + "\n";
                }
            }

            string d = "";
            for (int i = 0; i < dps.Count; i++) {
                if (i == dps.Count - 1) {
                    d += dps[i];
                } else {
                    d += dps[i] + "\n";
                }
            }

            if (t == "") {
                t = "- None";
            }
            if (h == "") {
                h = "- None";
            }
            if (d == "") {
                d = "- None";
            }

            var embed = await Task.Run(() => (new EmbedBuilder()
                .WithTitle(Utilities.UppercaseFirst(title))
                .WithDescription(Utilities.UppercaseFirst(description))
                .WithColor(Color.Green)
                .WithFields(
                    new EmbedFieldBuilder()
                        .WithName("When?")
                        .WithValue(dateTime)
                        .WithIsInline(false)
                    )
                .WithFields(
                    new EmbedFieldBuilder()
                        .WithName("Tanks")
                        .WithValue(t)
                        .WithIsInline(true)
                    )
                .WithFields(
                    new EmbedFieldBuilder()
                        .WithName("Healers")
                        .WithValue(h)
                        .WithIsInline(true)
                    )
                .WithFields(
                    new EmbedFieldBuilder()
                        .WithName("DPS")
                        .WithValue(d)
                        .WithIsInline(true)
                    )
                .WithThumbnailUrl("https://ffxiv.consolegameswiki.com/mediawiki/images/8/87/Main_Scenario_Quest_icon.png")
                .WithFooter(footer)
                .WithCurrentTimestamp().Build()));
            return embed;
        }

#endregion

        public static async Task<Embed> CreateCalendarEmbed(Dictionary<DateTime, string> events) {
            string[] days = { "", "", "", "" , "" , "", "" };

            DateTime date = DateTime.Today;

            for (int i = 0; i < 7; i++) {
                DateTime cur = date.AddDays(i);

                foreach (KeyValuePair<DateTime, string> e in events) {
                    // If days are the same
                    if (e.Key.DayOfYear == cur.DayOfYear) {
                        if (e.Key.ToString("HH:mm") == "00:00") {
                            days[i] += "\n" + e.Value + " | All Day";
                        } else {
                            days[i] += "\n" + e.Value + " | " + e.Key.ToString("HH:mm");
                        }
                    }
                }

                days[i] += " ";
            }

            var embed = await Task.Run(() => (new EmbedBuilder()
                    .WithTitle(Utilities.UppercaseFirst("Calendar"))
                    .WithColor(Color.Blue)
                    .WithFields(
                        new EmbedFieldBuilder()
                            .WithName("**" + date.DayOfWeek.ToString() + "** - " + date.ToString("MMMM") + " " + date.Day)
                            .WithValue("```" + days[0] + "```")
                            .WithIsInline(false)
                        )
                    .WithFields(
                        new EmbedFieldBuilder()
                            .WithName("**" + date.AddDays(1).DayOfWeek.ToString() + "** - " + date.AddDays(1).ToString("MMMM") + " " + date.AddDays(1).Day)
                            .WithValue("```" + days[1] + "```")
                            .WithIsInline(false)
                        )
                    .WithFields(
                        new EmbedFieldBuilder()
                            .WithName("**" + date.AddDays(2).DayOfWeek.ToString() + "** - " + date.AddDays(2).ToString("MMMM") + " " + date.AddDays(2).Day)
                            .WithValue("```" + days[2] + "```")
                            .WithIsInline(false)
                        )
                    .WithFields(
                        new EmbedFieldBuilder()
                            .WithName("**" + date.AddDays(3).DayOfWeek.ToString() + "** - " + date.AddDays(3).ToString("MMMM") + " " + date.AddDays(3).Day)
                            .WithValue("```" + days[3] + "```")
                            .WithIsInline(false)
                        )
                    .WithFields(
                        new EmbedFieldBuilder()
                            .WithName("**" + date.AddDays(4).DayOfWeek.ToString() + "** - " + date.AddDays(4).ToString("MMMM") + " " + date.AddDays(4).Day)
                            .WithValue("```" + days[4] + "```")
                            .WithIsInline(false)
                        )
                    .WithFields(
                        new EmbedFieldBuilder()
                            .WithName("**" + date.AddDays(5).DayOfWeek.ToString() + "** - " + date.AddDays(5).ToString("MMMM") + " " + date.AddDays(5).Day)
                            .WithValue("```" + days[5] + "```")
                            .WithIsInline(false)
                        )
                    .WithFields(
                        new EmbedFieldBuilder()
                            .WithName("**" + date.AddDays(6).DayOfWeek.ToString() + "** - " + date.AddDays(6).ToString("MMMM") + " " + date.AddDays(6).Day)
                            .WithValue("```" + days[6] + "```")
                            .WithIsInline(false)
                        )
                    .WithFooter("All times are in UTC")
                    .WithCurrentTimestamp().Build()));
            return embed;
        }

        public static async Task<Embed> CreateDataCentersEmbed(dynamic info) {

            var embed = new EmbedBuilder();
            embed.WithTitle("Data Centers");

            string dc = "";

            foreach (var datacenter in info) {
                dc += "**" + datacenter.ToString().Split('"')[1] + "**\n";
            }

            embed.WithDescription("Please specify your Data Center.\n\n" + dc);

            embed.WithColor(Color.Blue);

            embed.WithCurrentTimestamp();

            return embed.Build();
        }

        public static async Task<Embed> CreateServerStatusEmbed(List<string> servers, dynamic status, string dataCenter) {

            var embed = new EmbedBuilder();
            embed.WithTitle(dataCenter + " Server Status");

            int count = 0;
            int onlineCount = 0;
            string intro = "";
            string str = "";

            foreach (var s in status) {
                foreach (string server in servers) {
                    if (s.ToString().Contains(server)) {
                        string c = s.ToString().Substring(s.ToString().Length - 1);

                        if (c == "1") {
                            onlineCount += 1;
                            str += "\n" +  Config.pre.success + " **" + server + "**";
                            //embed.AddField(server, Config.pre.success + " Online", true);
                        } else {
                            str += "\n" + Config.pre.error + " **" + server + "**";
                            //embed.AddField(server, Config.pre.success + " Offline", true);
                        }

                        count += 1;

                    }
                }
            }

            if (count == 0) {
                embed.WithDescription("Server Status Unavailable");
            } else {
                intro = onlineCount + " Out of " + count + " servers are online.\n";
                embed.WithDescription(intro + str);
            }

            embed.WithColor(Color.Blue);

            embed.WithCurrentTimestamp();

            return embed.Build();
        }

        public static async Task<Embed> CreateMusicEmbed(string title, string uri) {
            bool isYt = false;
            string ytVideoId = "";
            if (uri.Contains("youtube.com"))
            {
                isYt = true;
                var uriObj = new Uri(uri);
                var q = HttpUtility.ParseQueryString(uriObj.Query);
                ytVideoId = q["v"];
            }

            if (isYt) {
                var embed = await Task.Run(() => (new EmbedBuilder()
                    .WithTitle(title)
                    .WithImageUrl("https://img.youtube.com/vi/" + ytVideoId + "/0.jpg")
                    .WithUrl(uri)
                    .WithColor(Color.Red)
                    .WithCurrentTimestamp().Build()));
                return embed;
            } else {
                var embed = await Task.Run(() => (new EmbedBuilder()
                .WithTitle(title)
                .WithColor(Color.Green)
                .WithUrl(uri)
                .WithCurrentTimestamp().Build()));
                return embed;
            }
        }

        public static async Task<Embed> CreateMusicQueueEmbed(string title, string uri)
        {
            bool isYt = false;
            string ytVideoId = "";
            if (uri.Contains("youtube.com"))
            {
                isYt = true;
                var uriObj = new Uri(uri);
                var q = HttpUtility.ParseQueryString(uriObj.Query);
                ytVideoId = q["v"];
            }

            if (isYt)
            {
                var embed = await Task.Run(() => (new EmbedBuilder()
                    .WithTitle(title)
                    .WithThumbnailUrl("https://img.youtube.com/vi/" + ytVideoId + "/0.jpg")
                    .WithColor(Color.Red)
                    .WithFields(
                    new EmbedFieldBuilder()
                        .WithName("Link")
                        .WithValue("[Here](" + uri + ")")
                    )
                    .WithCurrentTimestamp().Build()));
                return embed;
            }
            else
            {
                var embed = await Task.Run(() => (new EmbedBuilder()
                .WithTitle(title)
                .WithColor(Color.Green)
                .WithFields(
                    new EmbedFieldBuilder()
                        .WithName("Link")
                        .WithValue("[Here](" + uri + ")")
                    )
                .WithCurrentTimestamp().Build()));
                return embed;
            }
        }

        public static async Task<Embed> CreateFcInfoEmbed(dynamic info) {
            var embed = new EmbedBuilder();
            embed.WithTitle(info.FreeCompany.Name.ToString() + " <" + info.FreeCompany.Tag.ToString() + ">");
            embed.WithDescription(info.FreeCompany.Slogan.ToString());

            // If GC is Maelstrom
            if (info.FreeCompany.GrandCompany.ToString() == "Maelstrom")
            {
                embed.WithColor(new Color(155, 20, 39));
                embed.WithThumbnailUrl("https://ffxiv.gamerescape.com/w/images/thumb/0/02/The_Maelstrom_Flag.png/200px-The_Maelstrom_Flag.png");
                embed.WithImageUrl("https://ffxiv.gamerescape.com/w/images/thumb/0/02/The_Maelstrom_Flag.png/200px-The_Maelstrom_Flag.png");
            }

            // If GC is Immortal Flames
            if (info.FreeCompany.GrandCompany.ToString() == "Immortal Flames")
            {
                embed.WithColor(new Color(63, 62, 47));
                embed.WithThumbnailUrl("https://ffxiv.gamerescape.com/w/images/thumb/c/ca/The_Immortal_Flames_Flag.png/200px-The_Immortal_Flames_Flag.png");
                embed.WithImageUrl("https://ffxiv.gamerescape.com/w/images/thumb/c/ca/The_Immortal_Flames_Flag.png/200px-The_Immortal_Flames_Flag.png");
            }

            // If GC is Order of the Twin Adder
            if (info.FreeCompany.GrandCompany.ToString() == "Order of the Twin Adder")
            {
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

            return embed.Build();
        }

        public static async Task<Embed> CreateFcMembersInfoEmbed(dynamic info) {
            var embed = new EmbedBuilder();
            embed.WithTitle(info.FreeCompany.Name.ToString() + " <" + info.FreeCompany.Tag.ToString() + ">");
            embed.WithDescription(info.FreeCompany.Slogan.ToString());

            // If GC is Maelstrom
            if (info.FreeCompany.GrandCompany.ToString() == "Maelstrom")
            {
                embed.WithColor(new Color(155, 20, 39));
                embed.WithThumbnailUrl("https://ffxiv.gamerescape.com/w/images/thumb/0/02/The_Maelstrom_Flag.png/200px-The_Maelstrom_Flag.png");
            }

            // If GC is Immortal Flames
            if (info.FreeCompany.GrandCompany.ToString() == "Immortal Flames")
            {
                embed.WithColor(new Color(63, 62, 47));
                embed.WithThumbnailUrl("https://ffxiv.gamerescape.com/w/images/thumb/c/ca/The_Immortal_Flames_Flag.png/200px-The_Immortal_Flames_Flag.png");
            }

            // If GC is Order of the Twin Adder
            if (info.FreeCompany.GrandCompany.ToString() == "Order of the Twin Adder")
            {
                embed.WithColor(new Color(232, 181, 22));
                embed.WithThumbnailUrl("https://ffxiv.gamerescape.com/w/images/thumb/8/8b/The_Order_of_the_Twin_Adder_Flag.png/200px-The_Order_of_the_Twin_Adder_Flag.png");
            }

            embed.AddField("Active Members", info.FreeCompany.ActiveMemberCount.ToString()
            ).Build();

            foreach (var member in info.FreeCompanyMembers)
            {

                embed.AddField(member.Name.ToString(), "**Rank:** " + member.Rank.ToString(), true)
                .Build();

            }

            embed.WithCurrentTimestamp();
            embed.WithFooter("ID: " + info.FreeCompany.ID.ToString());

            return embed.Build();
        }
    }
}