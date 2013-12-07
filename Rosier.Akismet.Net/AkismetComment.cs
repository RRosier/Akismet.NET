using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rosier.Akismet.Net
{
    /// <summary>
    /// DTO, representing a single comment to be verified by Akismet.
    /// </summary>
    public class AkismetComment
    {
        /// <summary>
        /// Gets or sets the front page or home URL of the instance making the request. For a blog or wiki this ould be the front page.
        /// </summary>
        /// <remarks>
        /// Must be a full URI, including http://
        /// </remarks>
        /// TODO-rro: Is Required
        public Uri Blog { get; set; }

        /// <summary>
        /// Gets or sets the IP address of the comment submitter.
        /// </summary>
        /// TODO-rro: Is Required
        public string UserIp { get; set; }

        /// <summary>
        /// Gets or sets the User Agent fo the web browser submitting the comment.
        /// Typically the HTTP_USER_AGENT cgi variable.
        /// </summary>
        /// <remarks>
        /// Not to be confused with the user agent of your Akismet library.
        /// </remarks>
        /// TODO-rro: Is Required
        public string UserAgent { get; set; }

        /// <summary>
        /// Gets or sets the content of the HTTP_REFERER header.
        /// </summary>
        public string Referrer { get; set; }

        /// <summary>
        /// Gets or sets the permalink location of the entry the comment was submitted to.
        /// </summary>
        public string Permalink { get; set; }

        /// <summary>
        /// Gets or sets the type of the comment - one of the <see cref="CommentTypes"/> or a made up value like "registration".
        /// </summary>
        public string CommentType { get; set; }

        /// <summary>
        /// Gets or sets the name submitted with the comment.
        /// </summary>
        public string CommentAuthor { get; set; }

        /// <summary>
        /// Gets or sets the Email address submitted with the comment.
        /// </summary>
        public string CommentAuthorEmail { get; set; }

        /// <summary>
        /// Gets or sets the URL submitted with the comment.
        /// </summary>
        public string CommentAuthorUrl { get; set; }

        /// <summary>
        /// Gets or sets the content that was submitted.
        /// </summary>
        public string CommentContent { get; set; }

        /// <summary>
        /// To the URL string representing this comment instance.
        /// </summary>
        /// <returns>The comment details, formatted to be send to Akismet for verification.</returns>
        public string ToUrlString()
        {
            var queryString = string.Format("blog={0}&user_ip={1}&user_agent={2}&referrer={3}&permalink={4}&comment_type={5}" +
                "&comment_author={6}&comment_author_email={7}&comment_author_url={8}&comment_content={9}",
                Uri.EscapeDataString(this.Blog.ToString()),
                Uri.EscapeDataString(this.UserIp),
                Uri.EscapeDataString(this.UserAgent),
                Uri.EscapeDataString(this.Referrer),
                Uri.EscapeDataString(this.Permalink),
                Uri.EscapeDataString(this.CommentType),
                Uri.EscapeDataString(this.CommentAuthor),
                Uri.EscapeDataString(this.CommentAuthorEmail),
                Uri.EscapeDataString(this.CommentContent));

            return queryString;
        }
    }
}
