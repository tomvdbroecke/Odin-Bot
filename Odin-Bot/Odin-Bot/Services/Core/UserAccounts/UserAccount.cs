using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odin_Bot.Services.Core.UserAccounts {
    public class UserAccount {
        public ulong UserId { get; set; }
        public ulong Beans { get; set; }
        public ulong HasAskedForBeans { get; set; }
        public ulong XivCharId { get; set; }
    }
}
