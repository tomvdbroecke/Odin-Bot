using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Odin_Bot.Services.Core.UserAccounts;
using Discord;
using Odin_Bot.Handlers;

namespace Odin_Bot.Services {
    public class SchedulerService {
        private static SchedulerService _instance;
        private List<Timer> timers = new List<Timer>();

        private SchedulerService() { }

        private XivApiService _xivApiService;
        private CalendarService _calendarService;

        public SchedulerService(XivApiService xivApiService, CalendarService calendarService) {
            _xivApiService = xivApiService;
            _calendarService = calendarService;
        }

        public static SchedulerService Instance => _instance ?? (_instance = new SchedulerService());

        public Dictionary<DateTime, string> dailyEvents = new Dictionary<DateTime, string>();

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

        public async Task SetRandomGame(DiscordSocketClient client) {
            string[] game = {
                ".help",
                "with your Beans"
            };

            Random r = new Random();
            int randomIndex = r.Next(game.Length);
            await client.SetGameAsync(game[randomIndex]);
        }

        public async Task AnnounceEvents(DiscordSocketClient client) {
            try {
                foreach (KeyValuePair<DateTime, string> e in dailyEvents) {
                    if (Config.channels.eventAnnouncementChannel == 0)
                        return;

                    DateTime now = DateTime.Now;
                    TimeSpan ts = e.Key - now;

                    if (ts.TotalMinutes <= 10 && ts.TotalMinutes > 0) {
                        foreach (var guild in client.Guilds) {
                            SocketTextChannel channel = guild.GetTextChannel(Config.channels.eventAnnouncementChannel);
                            await channel.SendMessageAsync(e.Value + " is starting in " + Math.Round(ts.TotalMinutes) + " minutes!");
                        }

                        dailyEvents.Remove(e.Key);
                    }
                }
            } catch (Exception e) {
                await LoggingService.LogAsync("BOT", LogSeverity.Debug, e.Message);
            }
        }

        public async Task HourlyCalendarUpdate(DiscordSocketClient client) {
            Dictionary<DateTime, string> weeklyCalendar = _calendarService.GetNextWeekCalendar();
            DateTime date = DateTime.Today;

            // Populate "daily events" (also includes tomorrows' events in case of 24h overlap)
            dailyEvents = new Dictionary<DateTime, string>();
            foreach (KeyValuePair<DateTime, string> e in weeklyCalendar) {
                // Grab today and tomorrow
                if (e.Key.DayOfYear == date.DayOfYear || e.Key.AddDays(1).DayOfYear == date.AddDays(1).DayOfYear) {
                    if (e.Key.Minute >= 10)
                        dailyEvents.Add(e.Key, e.Value);
                }
            }

            // Refresh all calendars
            foreach (var guild in client.Guilds) {
                foreach (ulong calendarId in Config.calendarIdTracker) {
                    foreach (SocketGuildChannel channel in guild.Channels) {
                        if (channel.GetType() == typeof(SocketTextChannel)) {
                            ISocketMessageChannel ch = channel as ISocketMessageChannel;
                            IUserMessage message = await ch.GetMessageAsync(calendarId) as IUserMessage;

                            if (message != null) {
                                var embed = await EmbedHandler.CreateCalendarEmbed(weeklyCalendar);
                                await message.ModifyAsync(q => {
                                    q.Embed = embed;
                                });
                            }
                        }
                    }
                }
            }    
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
                // Grab highest level class

                double sinceJoined = Math.Round((DateTime.Now - a.AesirJoinDate).TotalDays);
                messageStr += "[" + a.FirstName + " " + a.LastName + "] [" + a.AesirRank + "] Joined #" + sinceJoined + " days ago.\n";
            }

            messageStr += "```";
            await channel.SendMessageAsync(messageStr);
        }

        public async Task ClearBeanRequests(DiscordSocketClient client) {
            List<UserAccount> accountList = UserAccounts.GetAllUserAccounts();
            Random random = new Random();

            // Grab bot channel
            ISocketMessageChannel channel;
            if (Config.channels.botChannels.Count() > 0) {
                channel = (ISocketMessageChannel)client.GetChannel(Config.channels.botChannels[0]);
            } else {
                await LoggingService.LogAsync("Bot", Discord.LogSeverity.Info, "There are no bot channels set, daily bean status update will not be sent.");
                return;
            }

            // Exit if mod channel is null
            if (channel == null) {
                return;
            }

            string beanStatus = "";

            foreach (UserAccount a in accountList) {
                if (a.HasAskedForBeans > 0) {
                    ulong b = (ulong)random.Next(10, 50);
                    a.Beans += b; // award beans each day

                    SocketGuildChannel ch = (SocketGuildChannel)channel;
                    SocketGuildUser u = ch.Guild.GetUser(a.UserId);
                    beanStatus += u.Username + " has tried to get beans " + a.HasAskedForBeans + " times today. They have been awarded " + b + " bonus beans.\n";

                    a.HasAskedForBeans = 0;
                }
            }

            await channel.SendMessageAsync("Here are your bonus beans HEATHENS!\n```" + beanStatus + "```");

            UserAccounts.SaveAccounts();
        }
    }
}
