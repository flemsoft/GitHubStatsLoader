using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using GitHubTest.Tests.Mocks;
using System.Threading.Tasks;
using GitHubTest.Models;
using System.Linq;
using GitHubTest.RepositoryData;

namespace GitHubTest.Tests
{
    [TestClass()]
    public class RepositoryInfoServiceTests
    {
        [TestMethod()]
        public async Task GetRepositoryInfos_SmallPageSize()
        {
            var allRepositories = new Repository[] {
                new RepositoryMock("repo1", "user1"),
                new RepositoryMock("repo2", "user1"),
                new RepositoryMock("repo3", "user1")
            };

            var allContributors = new Dictionary<string, Contributor[]>() {
                { "repo1", new Contributor[] {
                    new ContributorInfoMock("user5", 1),
                    new ContributorInfoMock("user6", 2),
                    new ContributorInfoMock("user7", 5) }
                }
            };

            IDataLoaderFactory factory = new DataLoaderFactoryMock(allRepositories, allContributors, 2);

            var loader = new RepositoryInfoService(factory);

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

            var allContributors = new Dictionary<string, Contributor[]>() {
                { "repo3", new Contributor[] {
                    new ContributorInfoMock("user5", 1),
                    new ContributorInfoMock("user6", 2),
                    new ContributorInfoMock("user7", 5) }
                }
            };

            IDataLoaderFactory factory = new DataLoaderFactoryMock(allRepositories, allContributors, 20);

            var loader = new RepositoryInfoService(factory);

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

            var allContributors = new Dictionary<string, Contributor[]>();

            IDataLoaderFactory factory = new DataLoaderFactoryMock(allRepositories, allContributors);

            var loader = new RepositoryInfoService(factory);

            var result = new List<RepositoryStatInfo>();
            await foreach (var repositoryInfo in loader.GetRepositoryInfosAsync("flemsoft"))
                result.Add(repositoryInfo);
        }
    }
}