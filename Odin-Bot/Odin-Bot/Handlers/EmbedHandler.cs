using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Odin_Bot;

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