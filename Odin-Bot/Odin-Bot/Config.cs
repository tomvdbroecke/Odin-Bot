﻿using System;
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
        private const string xivConfigFile = "xivConfig.json";
        private const string botMemoryFile = "botMemory.json";

        public static BotConfig bot;
        public static XivApiConfig xivApiConfig;
        public static Prefixes pre;
        public static BotMemory mem;

        // Config file constructor
        static Config() {
            // Set prefixes
            pre.error = ":no_entry_sign:";
            pre.success = ":white_check_mark:";

            // Create directory if it doesn't exist
            if (!Directory.Exists(configFolder)) Directory.CreateDirectory(configFolder);

            // Create file as json if it doesn't exist, read json file if it does exist
            /* BOTCONFIG */
            if (!File.Exists(configFolder + "/" + configFile)) {
                bot = new BotConfig();
                string json = JsonConvert.SerializeObject(bot, Formatting.Indented);
                File.WriteAllText(configFolder + "/" + configFile, json);
            } else {
                string json = File.ReadAllText(configFolder + "/" + configFile);
                bot = JsonConvert.DeserializeObject<BotConfig>(json);
            }

            /* XivApiConfig */
            if (!File.Exists(configFolder + "/" + xivConfigFile))
            {
                xivApiConfig = new XivApiConfig();
                string json = JsonConvert.SerializeObject(xivApiConfig, Formatting.Indented);
                File.WriteAllText(configFolder + "/" + xivConfigFile, json);
            }
            else
            {
                string json = File.ReadAllText(configFolder + "/" + xivConfigFile);
                xivApiConfig = JsonConvert.DeserializeObject<XivApiConfig>(json);
            }

            /* BotMemory */
            if (!File.Exists(configFolder + "/" + botMemoryFile))
            {
                mem = new BotMemory();
                string json = JsonConvert.SerializeObject(mem, Formatting.Indented);
                File.WriteAllText(configFolder + "/" + botMemoryFile, json);
            }
            else
            {
                string json = File.ReadAllText(configFolder + "/" + botMemoryFile);
                mem = JsonConvert.DeserializeObject<BotMemory>(json);
            }
        }

        public async Task SaveMemory() {
            string json = JsonConvert.SerializeObject(mem, Formatting.Indented);
            File.WriteAllText(configFolder + "/" + botMemoryFile, json);
        }
    }

    public struct BotConfig {
        public string token;
        public string cmdPrefix;
    }

    public struct XivApiConfig {
        public string xivapiKey;
        public string xivapiServer;
        public string fcLodestoneId;
    }

    public struct Prefixes {
        public string error;
        public string success;
    }

    public struct BotMemory {
        public int musicVolume;
    }
}
