using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Odin_Bot.Handlers;
using Odin_Bot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odin_Bot.Modules {
    public class CalendarModule : ModuleBase<SocketCommandContext> {
        private CalendarService _calendarService;

        public CalendarModule(CalendarService calendarService) {
            _calendarService = calendarService;
        }

        [Command("displaycalendar")]
        public async Task DisplayCalendar() {
            // REQUIRE MODERATOR
            if (!await PermissionService.RequireModerator(Context))
                return;

            // Send message and save ID
            Embed embed = await EmbedHandler.CreateCalendarEmbed(_calendarService.GetNextWeekCalendar());
            RestUserMessage msg = await Context.Channel.SendMessageAsync("@here", false, embed);
            Config.calendarIdTracker.Add(msg.Id);

            // Save message tracker
            var config = new Config();
            await config.SaveCalendarIdTracker();

            await Context.Message.DeleteAsync();
        }

        [Command("updatecalendars")]
        public async Task UpdateCalendars() {
            foreach (ulong calendarId in Config.calendarIdTracker) {
                foreach (SocketGuildChannel channel in Context.Guild.Channels) {
                    if (channel.GetType() == typeof(SocketTextChannel)) {
                        ISocketMessageChannel ch = channel as ISocketMessageChannel;

                        IUserMessage message = await ch.GetMessageAsync(calendarId) as IUserMessage;

                        if (message != null) {
                            var embed = await EmbedHandler.CreateCalendarEmbed(_calendarService.GetNextWeekCalendar());
                            await message.ModifyAsync(q => {
                                q.Embed = embed;
                            });
                        }
                    }
                }
            }

            await Context.Message.DeleteAsync();
        }
    }
}
