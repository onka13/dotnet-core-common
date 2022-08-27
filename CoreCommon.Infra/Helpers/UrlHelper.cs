using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CoreCommon.Infra.Helpers
{
    /// <summary>
    /// Url helpers
    /// </summary>
    public class UrlHelper
    {
        /// <summary>
        /// Checks the Url status without downloading data. 
        /// </summary>
        /// <param name="client">HttpClient object or null</param>
        /// <param name="url">An url to check</param>
        /// <returns>
        /// reason: response.ReasonPhrase or exception message
        /// status: -2: invalid url, -1: timeout, 0> response.StatusCode
        /// </returns>
        public static async Task<(string Reason, int Status)> CheckUrl(string url, HttpClient client = null)
        {
            bool dispose = client == null;
            if (client == null)
            {
                client = new HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(20)
                };
            }
            var cancellationToken = new CancellationToken();
            (string Reason, int Status) result = ("", 0);
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
                if (dispose) client.Dispose();
            }

            return result;
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
