using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Flurl.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Odin_Bot.Services;
using Odin_Bot.Services.Core.UserAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Odin_Bot.Modules {
    public class FunModule : ModuleBase<SocketCommandContext> {
        [Command("beans")]
        public async Task Beans() {
            UserAccount account = UserAccounts.GetAccount(Context.User);
            await ReplyAsync($"You currently have `{account.Beans}` beans.");
        }

        [Command("getbeans")]
        public async Task GetBeans() {
            UserAccount account = UserAccounts.GetAccount(Context.User);
            Random random = new Random();

            if (account.HasAskedForBeans == 0) {
                ulong beansGained = (ulong)random.Next(10, 50);
                account.Beans += beansGained;
                account.HasAskedForBeans += 1;
                await ReplyAsync($"I have blessed you with `{beansGained}` beans. You currently have `{account.Beans}` beans.");
            } else {
                float percentageRoll = (ulong)random.Next(101);
                float odinsBlessing = ((float)account.HasAskedForBeans / (2f + (float)account.HasAskedForBeans)) * 100f;

                if (odinsBlessing < percentageRoll) {
                    // Odin bless
                    ulong beansGained = (ulong)random.Next(10, 50);
                    account.Beans += beansGained;
                    account.HasAskedForBeans += 1;
                    await ReplyAsync($"You are in luck today! I have blessed you with `{beansGained}` more beans. You currently have `{account.Beans}` beans.");
                } else {
                    // Odin angery
                    ulong beansTaken = (ulong)random.Next((int)(account.HasAskedForBeans * 16));

                    if (beansTaken >= account.Beans) {
                        account.Beans = 0;
                    } else {
                        account.Beans -= beansTaken;
                    }

                    account.HasAskedForBeans += 1;
                    await ReplyAsync($"I HAVE ALREADY BLESSED YOU WITH BEANS TODAY HEATHEN! DO NOT ASK ME FOR MORE! I WILL TAKE BACK `{beansTaken}` BEANS! You now have `{account.Beans}` beans.");
                }
            }

            UserAccounts.SaveAccounts();
        }

        [Command("beanchance")]
        public async Task BeanChance() {
            UserAccount account = UserAccounts.GetAccount(Context.User);

            float odinsBlessing = ((float)account.HasAskedForBeans / (2f + (float)account.HasAskedForBeans)) * 100f;

            await ReplyAsync($"You currently have a `{100f - odinsBlessing}%` chance to get more beans today.");
        }

        [Command("beansleaderboard")]
        public async Task BeansLeaderboard() {
            Dictionary<string, ulong> beanDict = new Dictionary<string, ulong>();

            List<UserAccount> accountList = UserAccounts.GetAllUserAccounts();
            foreach (UserAccount a in accountList) {
                string username = "";
                foreach (SocketGuildUser u in Context.Guild.Users) {
                    if (u.Id == a.UserId)
                        username = u.Username;
                }

                if (username != "") {
                    beanDict.Add(username, a.Beans);
                }
            }

            string beanBoard = "";
            foreach (KeyValuePair<string, ulong> pair in beanDict.OrderByDescending(key => key.Value)) {
                beanBoard += pair.Key + ": " + pair.Value + "\n";
            }

            await ReplyAsync("The top 10 people with the most beans are:\n" +
            "```" + beanBoard +
            "```");
        }

        [Command("clearbeans")]
        public async Task ClearBeans() {
            // REQUIRE OWNER
            if (!await PermissionService.RequireOwner(Context))
                return;

            List<UserAccount> accountList = UserAccounts.GetAllUserAccounts();
            foreach (UserAccount a in accountList) {
                a.HasAskedForBeans = 0;
            }

            UserAccounts.SaveAccounts();
        }

        [Command("birb")]
        public async Task Birb() {
            string requestString = "https://some-random-api.ml/img/birb";
            HttpResponseMessage req = await requestString.GetAsync();

            dynamic info = JsonConvert.DeserializeObject(
                req.Content.ReadAsStringAsync().Result
            );

            await ReplyAsync(info.link.ToString());
        }

        /*
        [Command("talk")]
        public async Task Talk([Remainder]string query) {
            string requestString = "https://some-random-api.ml/chatbot?message=" + query;
            HttpResponseMessage req = await requestString.GetAsync();

            dynamic info = JsonConvert.DeserializeObject(
                req.Content.ReadAsStringAsync().Result
            );

            await ReplyAsync(Context.User.Mention + " " + info.response);
        }
        */
    }
}
