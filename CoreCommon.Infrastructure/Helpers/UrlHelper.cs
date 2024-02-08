using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CoreCommon.Infrastructure.Enums;

namespace CoreCommon.Infrastructure.Helpers
{
    /// <summary>
    /// Url helpers.
    /// </summary>
    public class UrlHelper
    {
        /// <summary>
        /// Checks the Url status without downloading data.
        /// </summary>
        /// <param name="url">An url to check.</param>
        /// <param name="client">HttpClient object or null.</param>
        /// <returns>
        /// reason: response.ReasonPhrase or exception message
        /// status: -2: invalid url, -1: timeout, 0> response.StatusCode.
        /// </returns>
        public static async Task<(string Reason, int Status)> CheckUrl(string url, HttpClient client = null)
        {
            bool dispose = client == null;
            if (client == null)
            {
                client = new HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(20),
                };
            }

            var cancellationToken = CancellationToken.None;
            (string Reason, int Status) result = (string.Empty, 0);
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Head, url);
                request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3904.97 Safari/537.36");

                using (var response = await client.SendAsync(request, cancellationToken))
                {
                    result.Reason = response.ReasonPhrase;
                    result.Status = (int)response.StatusCode;
                }
            }
            catch (InvalidOperationException)
            {
                result.Status = -2;
                result.Reason = "Invalid URL";
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken == cancellationToken)
                {
                    // a real cancellation, triggered by the caller (service stopped etc.)
                    // do nothing
                }
                else
                {
                    result.Status = -1;
                    result.Reason = "Timeout";

                    // a web request timeout (possibly other things!?)
                }
            }
            catch (Exception ex)
            {
                result.Reason = ex.Message;
            }
            finally
            {
                if (dispose)
                {
                    client.Dispose();
                }
            }

            return result;
        }

        /// <summary>
        /// DownloadContentAsync by Uri is get request.
        /// </summary>
        /// <param name="uri">API Endpoint.</param>
        /// <param name="mediaTypeHeaderValue">application/json, application/xml, text/json.</param>
        /// <param name="authenticationScheme">Enum AuthenticationScheme.</param>
        /// <param name="authenticationString">use this format to pass --> $"{CLIENT_ID}:{CLIENT_SEC}".</param>
        /// <returns></returns>
        public static async Task<string> DownloadContentAsync(string uri, string mediaTypeHeaderValue, AuthenticationScheme authenticationScheme, string authenticationString)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(mediaTypeHeaderValue));
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(authenticationScheme.ToString(), authenticationString);
                    using (HttpResponseMessage response = await client.GetAsync(uri))
                    {
                        response.EnsureSuccessStatusCode();
                        return await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        /// <summary>
        /// Append URL with params.
        /// </summary>
        /// <param name="baseUrl">The base URL/API which needs to append.</param>
        /// <param name="paramenterFields">Collection (Dictonary) of Parameter field.</param>
        /// <returns></returns>
        public static string AppendURLParam(string baseUrl, Dictionary<string, string> paramenterFields)
        {
            string url = string.Empty;

            url = baseUrl + "?";

            if (paramenterFields != null)
            {
                var fields = string.Empty;

                foreach (var field in paramenterFields)
                {
                    fields = fields + $"&{field.Key}={field.Value}";
                }

                url = url + fields;
            }

            return url;
        }

        public static string GetNiceFileName(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;
            foreach (char invalidchar in System.IO.Path.GetInvalidFileNameChars())
            {
                text = text.Replace(invalidchar, '_');
            }
            return text;
        }
    }
}
