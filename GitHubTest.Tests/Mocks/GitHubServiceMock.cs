using System;
using System.Collections.Generic;
using System.Text;
using GitHubTest.Interfaces;
using GitHubTest.Models;
using Octokit;

namespace GitHubTest.Tests.Mocks
{
    class GitHubServiceMock : IGitHubService
    {
        public GitHubServiceMock(IEnumerable<Repository> allRepositories, IReadOnlyDictionary<string, ContributorInfo[]> allContributors, int pageSize = 5)
        {
            CreateRepositoryDataLoader = (userLogin) =>
            {
                return new PageLoaderMock<Repository>(allRepositories)
                {
                    PageSize = pageSize
                };
            };

            CreateContributorDataLoader = (userLogin, repositoryName) =>
            {
                if (!allContributors.TryGetValue(repositoryName, out var contributors))
                    contributors = new ContributorInfo[] { };

                return new PageLoaderMock<ContributorInfo>(contributors)
                {
                    PageSize = pageSize
                };
            };
        }

        public Func<string, IDataLoader<Repository>> CreateRepositoryDataLoader { get; private set; }

        public IDataLoader<Repository> GetRepositoryDataLoader(string userLogin)
        {
            return CreateRepositoryDataLoader(userLogin);
        }

        public Func<string, string, IDataLoader<ContributorInfo>> CreateContributorDataLoader { get; private set; }

        public IDataLoader<ContributorInfo> GetContributorDataLoader(string userLogin, string repositoryName)
        {
            return CreateContributorDataLoader(userLogin, repositoryName);
        }
    }
}
