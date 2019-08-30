using Discord.Commands;
using Odin_Bot.Services.Core.UserAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odin_Bot.Modules {
    public class FunModule : ModuleBase<SocketCommandContext> {
        [Command("beans")]
        public async Task Beans() {
            UserAccount account = UserAccounts.GetAccount(Context.User);
            await ReplyAsync($"You currently have `{account.Beans}` beans.");
        }
    }
}
