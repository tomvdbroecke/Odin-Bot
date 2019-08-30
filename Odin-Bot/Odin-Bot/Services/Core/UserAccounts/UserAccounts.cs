using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odin_Bot.Services.Core.UserAccounts {
    public static class UserAccounts {
        private static List<UserAccount> accounts = null;

        private static string accountsFile = "Resources/accounts.json";

        static UserAccounts () {
            if (UserDataStorage.SaveExists(accountsFile)) {
                accounts = UserDataStorage.LoadUserAccounts(accountsFile).ToList();
            } else {
                accounts = new List<UserAccount>();
                SaveAccounts();
            }
        }

        public static void SaveAccounts() {
            UserDataStorage.SaveUserAccounts(accounts, accountsFile);
        }

        public static UserAccount GetAccount(SocketUser user) {
            return GetOrCreateAccount(user.Id);
        }

        private static UserAccount GetOrCreateAccount(ulong id) {
            var result = from a in accounts
                         where a.UserId == id
                         select a;

            var account = result.FirstOrDefault();
            if (account == null) account = CreateUserAccount(id);
            return account;
        }

        public static UserAccount CreateUserAccount(ulong id) {
            // Set new account default vars
            var newAccount = new UserAccount() {
                UserId = id,
                Beans = 0,
                XivCharId = 0
            };

            accounts.Add(newAccount);
            SaveAccounts();
            return newAccount;
        }
    }
}
