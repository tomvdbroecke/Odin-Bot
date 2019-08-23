using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odin_Bot.Services {
    public static class PermissionService {
        public static async Task<bool> IsOwner(SocketGuildUser user) {
            if (user.Id == user.Guild.OwnerId) {
                return true;
            }
            
            return false;
        }
    }
}
