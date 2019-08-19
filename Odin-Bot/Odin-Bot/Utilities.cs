using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace Odin_Bot {
    class Utilities {
        private static Dictionary<string, string> alerts;

        // Read from alerts.json and grab correct string
        static Utilities() {
            string json = File.ReadAllText("Systemlang/alerts.json");
            var data = JsonConvert.DeserializeObject<dynamic>(json);
            alerts = data.ToObject<Dictionary<string, string>>();
        }

        // Return corresponding string or empty string
        public static string GetAlert(string key) {
            if (alerts.ContainsKey(key)) return alerts[key];
            return "";
        }

        public static string UppercaseFirst(string s) {
            // Check for empty string.
            if (string.IsNullOrEmpty(s)) {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }
    }
}
