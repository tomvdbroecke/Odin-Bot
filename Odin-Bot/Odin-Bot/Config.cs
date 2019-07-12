using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace Odin_Bot {
    class Config {
        private const string configFolder = "Resources";
        private const string configFile = "config.json";

        public static BotConfig bot;

        // Config file constructor
        static Config() {
            // Create directory if it doesn't exist
            if (!Directory.Exists(configFolder)) Directory.CreateDirectory(configFolder);

            // Create file as json if it doesn't exist, read json file if it does exist
            if (!File.Exists(configFolder + "/" + configFile)) {
                bot = new BotConfig();
                string json = JsonConvert.SerializeObject(bot, Formatting.Indented);
                File.WriteAllText(configFolder + "/" + configFile, json);
            } else {
                string json = File.ReadAllText(configFolder + "/" + configFile);
                bot = JsonConvert.DeserializeObject<BotConfig>(json);
            }
        }
    }

    public struct BotConfig {
        public string token;
        public string cmdPrefix;
        public string xivapiKey;
        public string xivapiServer;
        public string fcLodestoneId;
    }
}
