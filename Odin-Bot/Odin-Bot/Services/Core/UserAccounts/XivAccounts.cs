using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odin_Bot.Services.Core.UserAccounts {
    public static class XivAccounts {
        private static List<XivAccount> xivAccounts = null;

        private static string accountsFile = "Resources/xivAccounts.json";

        static XivAccounts () {
            if (XivUserDataStorage.SaveExists(accountsFile)) {
                xivAccounts = XivUserDataStorage.LoadUserAccounts(accountsFile).ToList();
            } else {
                xivAccounts = new List<XivAccount>();
                SaveAccounts();
            }
        }

        public static void SaveAccounts() {
            XivUserDataStorage.SaveUserAccounts(xivAccounts, accountsFile);
        }

        public static void UpdateAccount(ulong charId, dynamic member) {
            for (int i = 0; i < xivAccounts.Count(); i++) {
                if (xivAccounts[i].CharId == charId) {
                    // UPDATE ACCOUNT VARS
                    string fullName = member.Name;
                    string[] splitName = fullName.Split(' ');
                    string server = member.Server;

                    xivAccounts[i].FirstName = splitName[0];
                    xivAccounts[i].LastName = splitName[1];
                    xivAccounts[i].Server = server;
                    xivAccounts[i].AesirRank = member.Rank;
                }
            }

            SaveAccounts();
        }

        public static void UpdateAccountList(List<XivAccount> list) {
            xivAccounts = list;
            SaveAccounts();
        }

        //public static XivAccount GetAccount(ulong charId, string firstName, string lastName, string server) {
            //return GetOrCreateAccount(charId, firstName, lastName, server);
        //}

        public static XivAccount GetOrCreateAccount(dynamic member) {
            var result = from a in xivAccounts
                         where a.CharId == Convert.ToUInt64(member.ID)
                         select a;

            var account = result.FirstOrDefault();
            if (account == null) account = CreateXivUserAccount(member);
            return account;
        }

        public static XivAccount CreateXivUserAccount(dynamic member) {
            // Set new account default vars
            string fullName = member.Name;
            string[] splitName = fullName.Split(' ');
            string server = member.Server;
            DateTime now = DateTime.Now;
            var newAccount = new XivAccount() {
                CharId = Convert.ToUInt64(member.ID),
                FirstName = splitName[0],
                LastName = splitName[1],
                Server = server,
                AesirJoinDate = now,
                AesirRank = member.Rank
            };

            xivAccounts.Add(newAccount);
            SaveAccounts();
            return newAccount;
        }
    }
}
