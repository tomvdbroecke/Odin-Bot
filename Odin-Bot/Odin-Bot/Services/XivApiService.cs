using Flurl.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Odin_Bot.Services;
using Discord;

namespace Odin_Bot.Services
{
    public class XivApiService {
        public async Task<dynamic> FCRequest(bool members = false) {
            try {
                string requestString = "";
                if (!members) {
                    requestString = "https://xivapi.com/freecompany/" + Config.xivApiConfig.fcLodestoneId + "?string=" + Config.xivApiConfig.xivapiServer + "&private_key=" + Config.xivApiConfig.xivapiKey;
                } else
                {
                    requestString = "https://xivapi.com/freecompany/" + Config.xivApiConfig.fcLodestoneId + "?data=FCM&string=" + Config.xivApiConfig.xivapiServer + "&private_key=" + Config.xivApiConfig.xivapiKey;
                }

                HttpResponseMessage req = await requestString.GetAsync();

                dynamic info = JsonConvert.DeserializeObject(
                    req.Content.ReadAsStringAsync().Result
                );

                return info;
            } catch (Exception e) {
                await LogAsync(new LogMessage(LogSeverity.Error, "XivApi", e.ToString()));

                return null;
            }
        }

        public async Task<dynamic> DataCenters() {
            try {
                string requestString = "https://xivapi.com/servers/dc";

                HttpResponseMessage req = await requestString.GetAsync();

                dynamic info = JsonConvert.DeserializeObject(
                    req.Content.ReadAsStringAsync().Result
                );

                return info;
            } catch (Exception e) {
                await LogAsync(new LogMessage(LogSeverity.Error, "XivApi", e.ToString()));

                return null;
            }
        }

        public async Task<dynamic> ServerStatus() {
            try {
                string requestString = "http://frontier.ffxiv.com/worldStatus/current_status.json";

                HttpResponseMessage req = await requestString.GetAsync();

                dynamic info = JsonConvert.DeserializeObject(
                    req.Content.ReadAsStringAsync().Result
                );

                return info;
            } catch (Exception e) {
                await LogAsync(new LogMessage(LogSeverity.Error, "XivApi", e.ToString()));

                return null;
            }
        }

        /*Used whenever we want to log something to the Console. */
        private async Task LogAsync(LogMessage logMessage)
        {
            await LoggingService.LogAsync(logMessage.Source, logMessage.Severity, logMessage.Message);
        }
    }
}
