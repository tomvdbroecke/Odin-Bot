using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;

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
                .WithDescription($"**Error Deaitls**: \n{error}")
                .WithColor(Color.DarkRed)
                .WithCurrentTimestamp().Build());
            return embed;
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

        public static async Task<Embed> CreateMusicQueueEmbed(string title string uri)
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
    }
}