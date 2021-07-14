using System.Collections.Generic;
using System.Linq;
using Jellyfin.Server.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;
using Xunit;

namespace Jellyfin.Server.Tests
{
    public static class UrlDecodeQueryFeatureTests
    {
        [Theory]
        [InlineData("e0a72cb2a2c7", "e0a72cb2a2c7")] // isn't encoded
        [InlineData("random+test", "random test")] // encoded
        [InlineData("random%20test", "random test")] // encoded
        [InlineData("++", "  ")] // encoded
        public static void EmptyValueTest(string query, string key)
        {
            var dict = new Dictionary<string, StringValues>
            {
                { query, StringValues.Empty }
            };
            var test = new UrlDecodeQueryFeature(new QueryFeature(new QueryCollection(dict)));
            Assert.Single(test.Query);
            var (k, v) = test.Query.First();
            Assert.Equal(key, k);
            Assert.Empty(v);
        }
    }
}
