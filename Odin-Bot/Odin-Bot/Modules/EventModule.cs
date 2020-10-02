using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Odin_Bot.Handlers;

namespace Odin_Bot.Modules {
    public class EventModule : ModuleBase<SocketCommandContext> {
        [Command("event")]
        public async Task Event([Remainder]string message) {
            // Check if format is correct
            string[] cmdVars = null;
            int maxUsers = 0;
            if (!message.Contains(";")) {
                await ReplyAsync(Config.pre.error + " When creating an event, please use the correct format: `" + Config.bot.cmdPrefix + "event [TITLE];[DESCRIPTION];[DATE+TIME];[MAX SIGNUPS (OPTIONAL)]`");
                // Remove user's message
                await Context.Message.DeleteAsync();
                return;
            } else {
                cmdVars = message.Split(';');
                if (cmdVars.Length > 4) {
                    await ReplyAsync(Config.pre.error + " You have entered too many parameters. Please use the correct format: `" + Config.bot.cmdPrefix + "event [TITLE];[DESCRIPTION];[DATE+TIME];[MAX SIGNUPS (OPTIONAL)]`");
                    // Remove user's message
                    await Context.Message.DeleteAsync();
                    return;
                }

                if (cmdVars.Length == 4) {
                    try {
                        maxUsers = Int32.Parse(cmdVars[3]);

                        if (maxUsers > 100) {
                            await ReplyAsync(Config.pre.error + " You can't have more than 100 signups per event.");
                            // Remove user's message
                            await Context.Message.DeleteAsync();
                            return;
                        }
                    } catch (Exception e) {
                        await ReplyAsync(Config.pre.error + " The format of the MAX SIGNUPS parameter is invalid. Please use the correct format: `" + Config.bot.cmdPrefix + "event [TITLE];[DESCRIPTION];[DATE+TIME];[MAX SIGNUPS (OPTIONAL)]`");
                        // Remove user's message
                        await Context.Message.DeleteAsync();
                        return;
                    }
                }
            }

            // Remove user's message
            await Context.Message.DeleteAsync();

            // Send message and save ID
            Embed embed = await EmbedHandler.CreateEventEmbed(cmdVars[0].Trim(), cmdVars[1].Trim(), maxUsers, cmdVars[2].Trim(), Context.User.Username);
            RestUserMessage msg = await Context.Channel.SendMessageAsync("@here", false, embed);
            Config.messageIdTracker.Add(msg.Id);

            // Add appropriate reactions
            var e1 = new Emoji("\u2705");
            await msg.AddReactionAsync(e1);
            var e2 = new Emoji("\u274C");
            await msg.AddReactionAsync(e2);
            
            // Save message tracker
            var config = new Config();
            await config.SaveMessageIdTracker();
        }

        [Command("lightpartyevent")]
        public async Task LightPartyEvent([Remainder]string message) {
            // Check if format is correct
            string[] cmdVars = null;
            int maxUsers = 0;
            if (!message.Contains(";")) {
                await ReplyAsync(Config.pre.error + " When creating an event, please use the correct format: `" + Config.bot.cmdPrefix + "lightpartyevent [TITLE];[DESCRIPTION];[DATE+TIME]`");
                // Remove user's message
                await Context.Message.DeleteAsync();
                return;
            } else {
                cmdVars = message.Split(';');
                if (cmdVars.Length > 3) {
                    await ReplyAsync(Config.pre.error + " You have entered too many parameters. Please use the correct format: `" + Config.bot.cmdPrefix + "lightpartyevent [TITLE];[DESCRIPTION];[DATE+TIME]`");
                    // Remove user's message
                    await Context.Message.DeleteAsync();
                    return;
                }
            }

            // Remove user's message
            await Context.Message.DeleteAsync();

            // Send message and save ID
            Embed embed = await EmbedHandler.CreateLightPartyEventEmbed(cmdVars[0].Trim(), cmdVars[1].Trim(), cmdVars[2].Trim(), Context.User.Username);
            RestUserMessage msg = await Context.Channel.SendMessageAsync("@here", false, embed);
            Config.messageIdTracker.Add(msg.Id);

            // Add appropriate reactions
            SocketGuild guild = Context.Guild;
            IEmote e1 = guild.Emotes.First(e => e.Name == "tank");
            await msg.AddReactionAsync(e1);
            IEmote e2 = guild.Emotes.First(e => e.Name == "healer");
            await msg.AddReactionAsync(e2);
            IEmote e3 = guild.Emotes.First(e => e.Name == "dps");
            await msg.AddReactionAsync(e3);

            // Save message tracker
            var config = new Config();
            await config.SaveMessageIdTracker();
        }

        [Command("fullpartyevent")]
        public async Task FullPartyEvent([Remainder]string message) {
            // Check if format is correct
            string[] cmdVars = null;
            int maxUsers = 0;
            if (!message.Contains(";")) {
                await ReplyAsync(Config.pre.error + " When creating an event, please use the correct format: `" + Config.bot.cmdPrefix + "fullpartyevent [TITLE];[DESCRIPTION];[DATE+TIME]`");
                // Remove user's message
                await Context.Message.DeleteAsync();
                return;
            } else {
                cmdVars = message.Split(';');
                if (cmdVars.Length > 3) {
                    await ReplyAsync(Config.pre.error + " You have entered too many parameters. Please use the correct format: `" + Config.bot.cmdPrefix + "fullpartyevent [TITLE];[DESCRIPTION];[DATE+TIME]`");
                    // Remove user's message
                    await Context.Message.DeleteAsync();
                    return;
                }
            }

            // Remove user's message
            await Context.Message.DeleteAsync();

            // Send message and save ID
            Embed embed = await EmbedHandler.CreateFullPartyEventEmbed(cmdVars[0].Trim(), cmdVars[1].Trim(), cmdVars[2].Trim(), Context.User.Username);
            RestUserMessage msg = await Context.Channel.SendMessageAsync("@here", false, embed);
            Config.messageIdTracker.Add(msg.Id);

            // Add appropriate reactions
            SocketGuild guild = Context.Guild;
            IEmote e1 = guild.Emotes.First(e => e.Name == "tank");
            await msg.AddReactionAsync(e1);
            IEmote e2 = guild.Emotes.First(e => e.Name == "healer");
            await msg.AddReactionAsync(e2);
            IEmote e3 = guild.Emotes.First(e => e.Name == "dps");
            await msg.AddReactionAsync(e3);

            // Save message tracker
            var config = new Config();
            await config.SaveMessageIdTracker();
        }
    }
}
