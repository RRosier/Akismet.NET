using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
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
        private readonly string applicationName = null;
        static private string pluginInfo = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Akismet" /> class.
        /// </summary>
        /// <param name="apiKey">The Akismet API key for use with the API.</param>
        /// <param name="blog">The blog.</param>
        /// <param name="applicationName">The application name and version {Application Name/Version}.</param>
        public Akismet(string apiKey, Uri blog, string applicationName)
        {
            ReadAssemblyInfo();

            this.apiKey = apiKey;
            this.blog = blog;
            this.applicationName = applicationName ?? pluginInfo;
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

            var client = CreateClient(false);

            var request = new HttpRequestMessage(HttpMethod.Post, AkismetUrls.VerifyKey);
            request.Content = new FormUrlEncodedContent(keyValues);

            var response = await client.SendAsync(request).ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                // TODO-rro: when false, save error message in Errors property.
                ////X-akismet-server: 192.168.6.48
                ////X-akismet-debug-help: We were unable to parse your blog URI
                return responseString.Equals("valid");
            }

            // TODO-rro: handle other status codes.

            return false;
        }

        /// <summary>
        /// Validtes the comment asynchronous.
        /// </summary>
        /// <param name="comment">The comment.</param>
        /// <returns>
        ///   <c>Spam</c> when the comment is spam, <c>Ham</c> when the comment is Ham aor <c>Invalid</c> when an error occured.
        /// </returns>
        public async Task<CommentCheck> CheckCommentAsync(AkismetComment comment)
        {
            var keyvalues = comment.CreateKeyValues();
            var client = this.CreateClient(true);
            var request = new HttpRequestMessage(HttpMethod.Post, AkismetUrls.ValidateComment);
            request.Content = new FormUrlEncodedContent(keyvalues);

            var response = await client.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseString = await response.Content.ReadAsStringAsync();

                switch (responseString)
                {
                    case "true": return CommentCheck.Spam;
                    case "false": return CommentCheck.Ham;
                    // TODO-rro: save error message in Errors property.
                    case "invalid": return CommentCheck.Invalid;
                    default: return CommentCheck.Invalid;
                }
            }

            return CommentCheck.Invalid;
        }

        /// <summary>
        /// Submits the comment as spam.
        /// </summary>
        /// <param name="comment">The comment.</param>
        /// <returns></returns>
        public async Task<bool> SubmitSpamAsync(AkismetComment comment)
        {
            var keyvalues = comment.CreateKeyValues();
            var client = this.CreateClient(true);
            var request = new HttpRequestMessage(HttpMethod.Post, AkismetUrls.SubmitSpam);
            request.Content = new FormUrlEncodedContent(keyvalues);

            var response = await client.SendAsync(request).ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                // TODO-rro: handle error message.
                return responseString.Equals("Thanks for making the web a better place.");
            }

            // TODO-rro: handle error message.
            return false;
        }

        /// <summary>
        /// Submits the comment as the ham.
        /// </summary>
        /// <param name="comment">The comment.</param>
        /// <returns></returns>
        public async Task<bool> SubmitHamAsync(AkismetComment comment)
        {
            var keyvalues = comment.CreateKeyValues();
            var client = this.CreateClient(true);
            var request = new HttpRequestMessage(HttpMethod.Post, AkismetUrls.SubmitHam);
            request.Content = new FormUrlEncodedContent(keyvalues);

            var response = await client.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                // TODO-rro: handle error message.
                return responseString.Equals("Thanks for making the web a better place.");
            }

            // TODO-rro: handle error message.
            return false;
        }

        /// <summary>
        /// Creates an <see cref="AkismetComment"/> object for the specified blog.
        /// </summary>
        /// <returns></returns>
        public AkismetComment CreateComment()
        {
            var comment = new AkismetComment(this.blog);
            return comment;
        }

        private HttpClient CreateClient(bool includeKey)
        {
            // Application Name/Version | Plugin/Version
            var userAgent = string.Format("{0} | {1}", this.applicationName, pluginInfo);
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

        private static void ReadAssemblyInfo()
        {
            if (pluginInfo != null)
                return;

            // TODO-rro: is their no better way to get this information from a portable class library?
            var assemblyFullName = Assembly.GetExecutingAssembly().FullName;

            var splitAssemblyName = assemblyFullName.Split(',');
            var assemblyName = splitAssemblyName[0].Trim();
            var version = splitAssemblyName[1].Split('=')[1];

            pluginInfo = string.Format("{0}/{1}", assemblyName, version);
        }
    }
}
