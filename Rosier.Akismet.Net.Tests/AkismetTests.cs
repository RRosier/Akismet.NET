using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rosier.Akismet.Net.Tests
{
    public class AkismetTests
    {
        private const string ApplicationName = "Rosier.Akismet.NET-Tests/1.0.0.0";

        [Fact]
        public async Task VerifyKey_Success()
        {
            var key = ConfigurationManager.AppSettings["apiKey"];

            var akismet = new Akismet(key, new Uri("http://www.mysite.com"), ApplicationName);
            var isValid = await akismet.VerifyKeyAsync();

            Assert.True(isValid);
        }

        [Fact]
        public async Task VerifyKey_Fail()
        {
            var key = "somefakekeyforunittesting";

            var akismet = new Akismet(key, new Uri("http://www.mysite.com"), ApplicationName);
            var isValid = await akismet.VerifyKeyAsync();

            Assert.False(isValid);
        }

        [Fact]
        public async Task CheckComment_Spam()
        {
            var key = ConfigurationManager.AppSettings["apiKey"];
            var akismet = new Akismet(key, new Uri("http://www.mysite.com"), ApplicationName);
            Assert.True(await akismet.VerifyKeyAsync());

            var comment = akismet.CreateComment();
            comment.UserIp = "127.0.0.1";
            comment.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1";
            comment.Referrer = "http://www.google.com";
            comment.Permalink = "/blog/post_one";
            comment.CommentType = CommentTypes.Comment;
            comment.CommentAuthor = "nike air max bw";
            comment.CommentAuthorEmail = "";
            comment.CommentAuthorUrl = "http://forum.fxdteam.com/profile/RetaWerth";
            comment.CommentContent = "Howdy! I simply would like to offer you a big thumbs up for your excellent info you have here on this post. I will be returning to your site for more soon.";

            var result = await akismet.CheckCommentAsync(comment);
            Assert.Equal(CommentCheck.Spam, result);
        }

        //[Fact]
        //public async Task CheckComment_KeyNotValidated()
        //{
        //    var comment = new AkismetComment()
        //    {
        //        Blog = new Uri("http://yourblogdomainname.com"),
        //        UserIp = "127.0.0.1",
        //        UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1",
        //        Referrer = "http://www.google.com",
        //        Permalink = "http://yourblogdomainname.com/blog/post_one",
        //        CommentType = CommentTypes.Comment,
        //        CommentAuthor = "nike air max bw",
        //        CommentAuthorEmail = "",
        //        CommentAuthorUrl = "http://forum.fxdteam.com/profile/RetaWerth",
        //        CommentContent = "Howdy! I simply would like to offer you a big thumbs up for your excellent info you have here on this post. I will be returning to your site for more soon."
        //    };

        //    var key = ConfigurationManager.AppSettings["apiKey"];
        //    var akismet = new Akismet(key, new Uri("http://www.mysite.com"), "Akismet.NET-Test/1.0");

        //    Assert.Throws(() => akismet.CheckCommentAsync(comment).Wait());
        //}
    }
}
