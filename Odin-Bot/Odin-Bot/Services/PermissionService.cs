using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odin_Bot.Services {
    public static class PermissionService {
        // Check if user is owner
        public static async Task<bool> IsOwner(SocketGuildUser user) {
            if (user.Id == user.Guild.OwnerId) {
                return true;
            }
            
            return false;
        }

        // Check if user is moderator
        public static async Task<bool> IsModerator(SocketGuildUser user) {
            if (Config.roles.moderators == null) {
                return false;
            }    

            foreach (var i in Config.roles.moderators) {
                var result = user.Guild.GetRole(i);
                if (result != null) {
                    if (result.Id == i) {
                        return true;
                    }
                }
            }

            return false;
        }

        // Moderator check
        public static async Task<bool> RequireModerator(SocketCommandContext Context) {
            if (await IsModerator((SocketGuildUser)Context.User) || await IsOwner((SocketGuildUser)Context.User)) {
                return true;
            } else {
                await Context.Channel.SendMessageAsync(Config.pre.error + " You do not have permission to use this command.");
                return false;
            }
        }

        // Owner check
        public static async Task<bool> RequireOwner(SocketCommandContext Context) {
            if (await IsOwner((SocketGuildUser)Context.User)) {
                return true;
            } else {
                await Context.Channel.SendMessageAsync(Config.pre.error + " You do not have permission to use this command.");
                return false;
            }
        }
    }
}
