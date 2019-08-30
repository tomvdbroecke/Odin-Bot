using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odin_Bot.Services.Core.UserAccounts {
    public class XivAccount {
        public ulong CharId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Server { get; set; }
        public DateTime AesirJoinDate { get; set; }
        public string AesirRank { get; set; }
    }
}
