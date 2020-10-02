using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Util.Store;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;

namespace Odin_Bot.Services {
    public class CalendarService {

        static string[] scopes = new string[] {
            Google.Apis.Calendar.v3.CalendarService.Scope.Calendar,
            Google.Apis.Calendar.v3.CalendarService.Scope.CalendarReadonly
        };

        static X509Certificate2 certificate = new X509Certificate2("Resources/odin-bot-291007-a5bbdfc7e28e.p12", "notasecret", X509KeyStorageFlags.Exportable);

        static ServiceAccountCredential credential = new ServiceAccountCredential(
            new ServiceAccountCredential.Initializer("odin-calendar@odin-bot-291007.iam.gserviceaccount.com") {
                Scopes = scopes
            }.FromCertificate(certificate));


        public Google.Apis.Calendar.v3.CalendarService service = new Google.Apis.Calendar.v3.CalendarService(new BaseClientService.Initializer() {
            HttpClientInitializer = credential,
            ApplicationName = "Odin Bot",
        });

        public Dictionary<DateTime, string> GetNextWeekCalendar() {
            var events = service.Events.List("t5prc7rp9hmm9u9krab4e2i46g@group.calendar.google.com").Execute();

            DateTime start = DateTime.Today;
            DateTime end = start.AddDays(7);

            Dictionary<DateTime, string> calendarEvents = new Dictionary<DateTime, string>();

            foreach (var e in events.Items) {
                if (e == null) {
                    continue;
                }

                bool pass = false;
                DateTime date;

                if (e.Start != null) {
                    if (e.Start.DateTime == null) {
                        DateTime.TryParse(e.Start.Date, out date);
                    } else {
                        DateTime.TryParse(e.Start.DateTimeRaw, out date);
                    }

                    if (date != null) {
                        if (IsBetween(date, start.Ticks, end.Ticks)) {
                            pass = true;
                        }
                    }

                    if (pass) {
                        calendarEvents.Add(date, e.Summary);
                    }
                }
            }

            return calendarEvents;
        }

        bool IsBetween(DateTime dateTime, long s, long e) {
            long now = dateTime.Ticks;

            return (s < now && now < e);
        }

    }
}
