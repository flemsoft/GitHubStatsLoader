using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using GitHubTest.Tests.Mocks;
using System.Threading.Tasks;
using Octokit;
using GitHubTest.Models;
using System.Linq;
using GitHubTest.Interfaces;

namespace GitHubTest.Tests
{
    [TestClass()]
    public class RepositoryInfoLoaderTests
    {
        [TestMethod()]
        public async Task GetRepositoryInfos_SmallPageSize()
        {
            var allRepositories = new Repository[] {
                new RepositoryMock("repo1", "user1"),
                new RepositoryMock("repo2", "user1"),
                new RepositoryMock("repo3", "user1")
            };

            var allContributors = new Dictionary<string, ContributorInfo[]>() {
                { "repo1", new ContributorInfo[] {
                    new ContributorInfoMock("user5", 1),
                    new ContributorInfoMock("user6", 2),
                    new ContributorInfoMock("user7", 5) }
                }
            };

            IGitHubService dataService = new GitHubServiceMock(allRepositories, allContributors, 2);

            var loader = new RepositoryInfoLoader(dataService);

            var result = new List<RepositoryStatInfo>();
            await foreach (var repositoryInfo in loader.GetRepositoryInfosAsync("flemsoft"))
                result.Add(repositoryInfo);

            CollectionAssert.AreEquivalent(allRepositories, result.Select(x => x.Repository).ToList());

            var repo1 = result.FirstOrDefault(x => x.Repository.Name == "repo1");
            Assert.IsNotNull(repo1);
            CollectionAssert.AreEquivalent(allContributors["repo1"], repo1.Contributors.ToList());
        }

        [TestMethod()]
        public async Task GetRepositoryInfos_LargePageSize()
        {
            var allRepositories = new Repository[] {
                new RepositoryMock("repo1", "user1"),
                new RepositoryMock("repo2", "user1"),
                new RepositoryMock("repo3", "user1")
            };

            var allContributors = new Dictionary<string, ContributorInfo[]>() {
                { "repo3", new ContributorInfo[] {
                    new ContributorInfoMock("user5", 1),
                    new ContributorInfoMock("user6", 2),
                    new ContributorInfoMock("user7", 5) }
                }
            };

            IGitHubService dataService = new GitHubServiceMock(allRepositories, allContributors, 20);

            var loader = new RepositoryInfoLoader(dataService);

            var result = new List<RepositoryStatInfo>();
            await foreach (var repositoryInfo in loader.GetRepositoryInfosAsync("flemsoft"))
                result.Add(repositoryInfo);

            CollectionAssert.AreEquivalent(allRepositories, result.Select(x => x.Repository).ToList());

            var repo1 = result.FirstOrDefault(x => x.Repository.Name == "repo3");
            Assert.IsNotNull(repo1);
            CollectionAssert.AreEquivalent(allContributors["repo3"], repo1.Contributors.ToList());
        }

        [TestMethod()]
        [ExpectedException(typeof(AggregateException))]
        public async Task GetRepositoryInfos_ThrowInvalidGitHubResponse()
        {
            var allRepositories = new Repository[] {
                new RepositoryMock("repo1", "user1"),
                new RepositoryMock("repo2", null),
                new RepositoryMock("repo3", "user1")
            };

            var allContributors = new Dictionary<string, ContributorInfo[]>();

            IGitHubService dataService = new GitHubServiceMock(allRepositories, allContributors);

            var loader = new RepositoryInfoLoader(dataService);

            var result = new List<RepositoryStatInfo>();
            await foreach (var repositoryInfo in loader.GetRepositoryInfosAsync("flemsoft"))
                result.Add(repositoryInfo);
        }
    }
}