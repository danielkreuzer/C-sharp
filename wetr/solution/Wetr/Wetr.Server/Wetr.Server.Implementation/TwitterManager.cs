using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Wetr.Domain;
using Wetr.Server.Interface;
using Timer = System.Timers.Timer;

namespace Wetr.Server.Implementation {
    public class TwitterManager : ITwitterManager {
        private class StationCheck {
            public int StationId { get; set; }
            public bool Visited { get; set; }

            public StationCheck(int stationId) {
                StationId = stationId;
                Visited = false;
            }
        }

        private static class Settings {
            public static int Temperature { get; set; } = 1;
            public static int Rainfall { get; set; } = 3;
            public static int Windspeed { get; set; } = 5;
        }

        private Timer timer;
        private List<StationCheck> stationChecks;

        public TwitterManager() {
            this.timer = new Timer(1000 * 60 * 5); // 1sec * 60 * 5 = 5min
            this.timer.Elapsed += TimerCheck;
            this.timer.AutoReset = true;
            this.timer.Enabled = true;
            stationChecks = new List<StationCheck>();
        }

        private void TimerCheck(object sender, ElapsedEventArgs e) {
            List<StationCheck> toDeleteChecks = new List<StationCheck>();
            foreach (StationCheck stationCheck in stationChecks) {
                if (stationCheck.Visited) {
                    toDeleteChecks.Add(stationCheck);
                }
                else {
                    stationCheck.Visited = true;
                }
            }

            foreach (StationCheck stationCheck in toDeleteChecks) {
                stationChecks.Remove(stationCheck);
            }
        }

        private bool StationCheckContains(int stationId) {
            foreach (StationCheck stationCheck in stationChecks) {
                if (stationCheck.StationId == stationId) {
                    return true;
                }
            }

            return false;
        }


        public async Task SendAlarmTweet(Measurement measurement) {
            if (CheckIfAlarmTweetNecessary(measurement)) {
                TwitterApi twitterApi = new TwitterApi();
                String messageText = await GenerateAlarmText(measurement);
                if (!messageText.Equals("")) {
                    await twitterApi.Tweet(messageText);
                }
            }
        }

        private async Task<String> GenerateAlarmText(Measurement measurement) {
            Station station = null;
            int typeId = 0;

            typeId = measurement.MeasurementType?.Id ?? measurement.TypeId;

            if (measurement.Station != null) {
                station = measurement.Station;
            }
            else {
                IStationDataManager stationDataManager = new StationDataManager();
                station = await stationDataManager.GetStationById(measurement.StationId);
            }

            if (!StationCheckContains(station.Id)) {

                stationChecks.Add(new StationCheck(station.Id));

                if (typeId == Settings.Temperature) {
                    return "Wetr temperature warning! Temperature " + measurement.Value + "°C at station " +
                           station.Name +
                           " in " +
                           station.Community.ZipCode + " " + station.Community.Name;
                }

                if (typeId == Settings.Rainfall) {
                    return "Wetr rainfall warning! Rainfall " + measurement.Value + "l/m² at station "  + station.Name +
                           " in " +
                           station.Community.ZipCode + " " + station.Community.Name;
                }

                if (typeId == Settings.Windspeed) {
                    return "Wetr windspeed warning! Windspeed " + measurement.Value + "km/h at station "  + station.Name +
                           " in " +
                           station.Community.ZipCode + " " + station.Community.Name;
                }
            }

            return "";
        }

        private bool CheckIfAlarmTweetNecessary(Measurement measurement) {
            int typeId = 0;

            typeId = measurement.MeasurementType?.Id ?? measurement.TypeId;

            if (typeId == Settings.Temperature) { // Temperature
                if (measurement.Value > 40 || measurement.Value < -19) {
                    return true;
                }

                return false;
            } else if (typeId == Settings.Rainfall) { // Rainfall
                if (measurement.Value > 10) {
                    return true;
                }

                return false;
            } else if (typeId == Settings.Windspeed) { // Windspeed
                if (measurement.Value > 60) {
                    return true;
                }

                return false;
            }

            return false;
        }
    }

    /// <summary>
    /// Simple class for sending tweets to Twitter using Single-user OAuth.
    /// https://dev.twitter.com/oauth/overview/single-user
    /// FROM
    /// https://blog.dantup.com/2016/07/simplest-csharp-code-to-post-a-tweet-using-oauth/
    /// 
    /// Get your access keys by creating an app at apps.twitter.com then visiting the
    /// "Keys and Access Tokens" section for your app. They can be found under the
    /// "Your Access Token" heading.
    /// </summary>
    class TwitterApi {
        const string TwitterApiBaseUrl = "https://api.twitter.com/1.1/";
        readonly string consumerKey, consumerKeySecret, accessToken, accessTokenSecret;
        readonly HMACSHA1 sigHasher;
        readonly DateTime epochUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Creates an object for sending tweets to Twitter using Single-user OAuth.
        /// 
        /// Get your access keys by creating an app at apps.twitter.com then visiting the
        /// "Keys and Access Tokens" section for your app. They can be found under the
        /// "Your Access Token" heading.
        /// </summary>
        public TwitterApi() {
            this.consumerKey = "";
            this.consumerKeySecret = "";
            this.accessToken = "";
            this.accessTokenSecret = "";

            sigHasher = new HMACSHA1(new ASCIIEncoding().GetBytes(string.Format("{0}&{1}", consumerKeySecret, accessTokenSecret)));
        }

        /// <summary>
        /// Sends a tweet with the supplied text and returns the response from the Twitter API.
        /// </summary>
        public Task<string> Tweet(string text) {
            var data = new Dictionary<string, string> {
            { "status", text },
            { "trim_user", "1" }
        };

            return SendRequest("statuses/update.json", data);
        }

        Task<string> SendRequest(string url, Dictionary<string, string> data) {
            var fullUrl = TwitterApiBaseUrl + url;

            // Timestamps are in seconds since 1/1/1970.
            var timestamp = (int)((DateTime.UtcNow - epochUtc).TotalSeconds);

            // Add all the OAuth headers we'll need to use when constructing the hash.
            data.Add("oauth_consumer_key", consumerKey);
            data.Add("oauth_signature_method", "HMAC-SHA1");
            data.Add("oauth_timestamp", timestamp.ToString());
            data.Add("oauth_nonce", "a"); // Required, but Twitter doesn't appear to use it, so "a" will do.
            data.Add("oauth_token", accessToken);
            data.Add("oauth_version", "1.0");

            // Generate the OAuth signature and add it to our payload.
            data.Add("oauth_signature", GenerateSignature(fullUrl, data));

            // Build the OAuth HTTP Header from the data.
            string oAuthHeader = GenerateOAuthHeader(data);

            // Build the form data (exclude OAuth stuff that's already in the header).
            var formData = new FormUrlEncodedContent(data.Where(kvp => !kvp.Key.StartsWith("oauth_")));

            return SendRequest(fullUrl, oAuthHeader, formData);
        }

        /// <summary>
        /// Generate an OAuth signature from OAuth header values.
        /// </summary>
        string GenerateSignature(string url, Dictionary<string, string> data) {
            var sigString = string.Join(
                "&",
                data
                    .Union(data)
                    .Select(kvp => string.Format("{0}={1}", Uri.EscapeDataString(kvp.Key), Uri.EscapeDataString(kvp.Value)))
                    .OrderBy(s => s)
            );

            var fullSigData = string.Format(
                "{0}&{1}&{2}",
                "POST",
                Uri.EscapeDataString(url),
                Uri.EscapeDataString(sigString.ToString())
            );

            return Convert.ToBase64String(sigHasher.ComputeHash(new ASCIIEncoding().GetBytes(fullSigData.ToString())));
        }

        /// <summary>
        /// Generate the raw OAuth HTML header from the values (including signature).
        /// </summary>
        string GenerateOAuthHeader(Dictionary<string, string> data) {
            return "OAuth " + string.Join(
                ", ",
                data
                    .Where(kvp => kvp.Key.StartsWith("oauth_"))
                    .Select(kvp => string.Format("{0}=\"{1}\"", Uri.EscapeDataString(kvp.Key), Uri.EscapeDataString(kvp.Value)))
                    .OrderBy(s => s)
            );
        }

        /// <summary>
        /// Send HTTP Request and return the response.
        /// </summary>
        async Task<string> SendRequest(string fullUrl, string oAuthHeader, FormUrlEncodedContent formData) {
            using (var http = new HttpClient()) {
                http.DefaultRequestHeaders.Add("Authorization", oAuthHeader);

                var httpResp = await http.PostAsync(fullUrl, formData);
                var respBody = await httpResp.Content.ReadAsStringAsync();

                return respBody;
            }
        }
    }

}
