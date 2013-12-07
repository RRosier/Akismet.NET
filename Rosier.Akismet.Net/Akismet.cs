using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Rosier.Akismet.Net
{
    /// <summary>
    /// Performs actions to validate comments against the Akismet service.
    /// </summary>
    public class Akismet
    {
        private readonly string apiKey;
        private readonly Uri blog;
        private readonly string applicationName;

        private bool keyVerified = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Akismet" /> class.
        /// </summary>
        /// <param name="apiKey">The Akismet API key for use with the API.</param>
        /// <param name="blog">The blog.</param>
        /// <param name="applicationName">The application name and version {Application Name/Version}.</param>
        public Akismet(string apiKey, Uri blog, string applicationName)
        {
            this.apiKey = apiKey;
            this.blog = blog;
            this.applicationName = applicationName ?? "Akismet.Net/1.0";
        }

        /// <summary>
        /// Verifies the key asynchronous.
        /// </summary>
        /// <returns><c>true</c> if the key is valid, else <c>false</c>.</returns>
        public async Task<bool> VerifyKeyAsync()
        {
            var keyValues = new List<KeyValuePair<string, string>>();
            keyValues.Add(new KeyValuePair<string, string>("key", this.apiKey));
            keyValues.Add(new KeyValuePair<string, string>("blog", this.blog.ToString()));

            var path = "/1.1/verify-key";

            var client = CreateClient(false);

            var request = new HttpRequestMessage(HttpMethod.Post, path);
            request.Content = new FormUrlEncodedContent(keyValues);

            var response = await client.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                // TODO-rro: when false, save error message in Errors property.
                ////X-akismet-server: 192.168.6.48
                ////X-akismet-debug-help: We were unable to parse your blog URI
                this.keyVerified = responseString.Equals("valid");
            }

            // TODO-rro: handle other status codes.

            return this.keyVerified;
        }

        private HttpClient CreateClient(bool includeKey)
        {
            // Application Name/Version | Plugin/Version
            var userAgent = string.Format("{0} | Akismet.Net/0.1", this.applicationName);
            string uriPrefix = string.Empty;
            if (includeKey)
            {
                uriPrefix = this.apiKey + ".";
            }

            Uri baseUri = new Uri(String.Format("http://{0}rest.akismet.com", uriPrefix));

            var handler = new HttpClientHandler();
            var client = new HttpClient(handler);
            client.BaseAddress = baseUri;
            client.DefaultRequestHeaders.Add("User-Agent", userAgent);

            return client;
        }
    }
}
