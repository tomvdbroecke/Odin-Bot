using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Odin_Bot.Services.Core.UserAccounts;

namespace Odin_Bot.Services {
    public class SchedulerService {
        private static SchedulerService _instance;
        private List<Timer> timers = new List<Timer>();

        private SchedulerService() { }
        private XivApiService _xivApiService;

        public SchedulerService(XivApiService xivApiService) => _xivApiService = xivApiService;

        public static SchedulerService Instance => _instance ?? (_instance = new SchedulerService());

        public void ScheduleTask(int hour, int min, double intervalInHour, Action task) {
            DateTime now = DateTime.Now;
            DateTime firstRun = new DateTime(now.Year, now.Month, now.Day, hour, min, 0, 0);
            if (now > firstRun) {
                firstRun = firstRun.AddDays(1);
            }
            TimeSpan timeToGo = firstRun - now;
            if (timeToGo <= TimeSpan.Zero) {
                timeToGo = TimeSpan.Zero;
            }
            var timer = new Timer(x =>
            {
                task.Invoke();
            }, null, timeToGo, TimeSpan.FromHours(intervalInHour));
            timers.Add(timer);
        }

        public async Task DailyModeratorReport(DiscordSocketClient client) {
            // Grab moderator channel
            ISocketMessageChannel channel;
            if (Config.channels.moderatorChannel != 0) {
                channel = (ISocketMessageChannel)client.GetChannel(Config.channels.moderatorChannel);
            } else {
                await LoggingService.LogAsync("Bot", Discord.LogSeverity.Info, "The moderator channel was not set. Daily moderator report can't be executed.");
                return;
            }

            // Exit if mod channel is null
            if (channel == null) {
                await LoggingService.LogAsync("Bot", Discord.LogSeverity.Info, "The moderator channel could not be found. Daily moderator report can't be executed.");
                return;
            }

            // Get FC member info
            dynamic info = await _xivApiService.FCRequest(true);
            // Get user account for each user
            List<XivAccount> accounts = new List<XivAccount>();
            try {
                foreach (var member in info.FreeCompanyMembers) {
                    XivAccount account = XivAccounts.GetOrCreateAccount(member);

                    XivAccounts.UpdateAccount(account.CharId, member);

                    accounts.Add(account);
                }
            } catch (Exception e) {
                await channel.SendMessageAsync(Config.pre.error + " An error has occured: " + e.Message);
            }

            // Check for accounts that don't exist anymore
            foreach (var a in accounts) {
                bool found = false;
                foreach (var member in info.FreeCompanyMembers) {
                    if (a.CharId == Convert.ToUInt64(member.ID)) {
                        found = true;
                    }
                }

                if (!found) {
                    accounts.Remove(a);
                }
            }
            XivAccounts.UpdateAccountList(accounts);

            // Create message string
            string messageStr = ":notepad_spiral: **Daily Report** @here\n" +
                                "```css\n";
            var sortedAccounts = accounts.OrderBy(x => x.AesirJoinDate);
            foreach (var a in sortedAccounts) {
                double sinceJoined = Math.Round((DateTime.Now - a.AesirJoinDate).TotalDays);
                messageStr += "[" + a.FirstName + " " + a.LastName + "] #" + a.AesirRank + " Joined #" + sinceJoined + " days ago.\n";
            }

            messageStr += "```";
            await channel.SendMessageAsync(messageStr);
        }
    }
}
