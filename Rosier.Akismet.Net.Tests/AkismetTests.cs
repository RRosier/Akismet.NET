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
        [Fact]
        public async Task VerifyKey_Success()
        {
            var key = ConfigurationManager.AppSettings["apiKey"];

            var akismet = new Akismet(key, new Uri("http://www.mysite.com"), "Akismet.NET-Test/1.0");
            var isValid = await akismet.VerifyKeyAsync();

            Assert.True(isValid);
        }

        [Fact]
        public async Task VerifyKey_Fail()
        {
            var key = "somefakekeyforunittesting";

            var akismet = new Akismet(key, new Uri("http://www.mysite.com"), "Akismet.NET-Test/1.0");
            var isValid = await akismet.VerifyKeyAsync();

            Assert.False(isValid);
        }
    }
}
