using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Odin_Bot.Services.Core.UserAccounts;
using Newtonsoft.Json;
using System.IO;

namespace Odin_Bot.Services.Core {
    public static class UserDataStorage {
        // Save all user accounts
        public static void SaveUserAccounts(IEnumerable<UserAccount> accounts, string filePath){
            string json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        // Get all user accounts
        public static IEnumerable<UserAccount> LoadUserAccounts(string filePath) {
            if (!File.Exists(filePath)) return null;
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<UserAccount>>(json);
        }

        // Check if accounts file exists
        public static bool SaveExists(string filePath) {
            return File.Exists(filePath);
        }
    }
}
