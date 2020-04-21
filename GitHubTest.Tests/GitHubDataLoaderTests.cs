using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using GitHubTest.Models;
using GitHubTest.RepositoryData;

namespace GitHubTest.Tests
{
    [TestClass()]
    public class GitHubDataLoaderTests
    {
        [TestMethod()]
        public void FindNextPageUri_HasNextPage()
        {
            var headers = new Dictionary<string, string>()
            {
                { "Access-Control-Expose-Headers", "ETag, Link, Location, Retry-After, X-GitHub-OTP, X-RateLimit-Limit, X-RateLimit-Remaining, X-RateLimit-Reset, X-OAuth-Scopes, X-Accepted-OAuth-Scopes, X-Poll-Interval, X-GitHub-Media-Type, Deprecation, Sunset" },
                { "Link", "<https://api.github.com/user/6154722/repos?page=20>; rel=\"next\", <https://api.github.com/user/6154722/repos?page=103>; rel=\"last\"" },
                { "Cache-Control", "max-age=60, s-maxage=60, private" },
            };
            var nextPageUri = GitHubDataLoader<RepositoryStatInfo>.FindNextPageUri(headers);

            Assert.AreEqual(nextPageUri, "https://api.github.com/user/6154722/repos?page=20");
        }

        [TestMethod()]
        public void FindNextPageUri_GetLastPage()
        {
            var headers = new Dictionary<string, string>()
            {
                { "Access-Control-Expose-Headers", "ETag, Link, Location, Retry-After, X-GitHub-OTP, X-RateLimit-Limit, X-RateLimit-Remaining, X-RateLimit-Reset, X-OAuth-Scopes, X-Accepted-OAuth-Scopes, X-Poll-Interval, X-GitHub-Media-Type, Deprecation, Sunset" },
                { "Link", "<https://api.github.com/user/6154722/repos?page=1>; rel=\"prev\", <https://api.github.com/user/6154722/repos?page=103>; rel=\"last\", <https://api.github.com/user/6154722/repos?page=1>; rel=\"first\"" },
                { "Cache-Control", "max-age=60, s-maxage=60, private" },
            };
            var nextPageUri = GitHubDataLoader<RepositoryStatInfo>.FindNextPageUri(headers);

            Assert.IsNull(nextPageUri);
        }

        [TestMethod()]
        public void FindNextPageUri_NoLinkHeader()
        {
            var headers = new Dictionary<string, string>()
            {
                { "Access-Control-Expose-Headers", "ETag, Link, Location, Retry-After, X-GitHub-OTP, X-RateLimit-Limit, X-RateLimit-Remaining, X-RateLimit-Reset, X-OAuth-Scopes, X-Accepted-OAuth-Scopes, X-Poll-Interval, X-GitHub-Media-Type, Deprecation, Sunset" },
                { "Link", "<https://api.github.com/user/6154722/repos?page=1>; rel=\"prev\", <https://api.github.com/user/6154722/repos?page=103>; rel=\"last\", <https://api.github.com/user/6154722/repos?page=1>; rel=\"first\"" },
                { "Cache-Control", "max-age=60, s-maxage=60, private" },
            };
            var nextPageUri = GitHubDataLoader<RepositoryStatInfo>.FindNextPageUri(headers);

            Assert.IsNull(nextPageUri);
        }
    }
}