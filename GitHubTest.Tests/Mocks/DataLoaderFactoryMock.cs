using System;
using System.Collections.Generic;
using GitHubTest.Models;
using GitHubTest.RepositoryData;

namespace GitHubTest.Tests.Mocks
{
    class DataLoaderFactoryMock : IDataLoaderFactory
    {
        public DataLoaderFactoryMock(IEnumerable<Repository> allRepositories, IReadOnlyDictionary<string, Contributor[]> allContributors, int pageSize = 5)
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
                    contributors = new Contributor[] { };

                return new PageLoaderMock<Contributor>(contributors)
                {
                    PageSize = pageSize
                };
            };
        }

        public IAsyncEnumerable<RepositoryStatInfo> GetRepositoryInfosAsync(string userLogin)
        {
            throw new NotImplementedException();
        }

        public Func<string, IDataLoader<Repository>> CreateRepositoryDataLoader { get; private set; }

        public IDataLoader<Repository> GetRepositoryDataLoader(string userLogin)
        {
            return CreateRepositoryDataLoader(userLogin);
        }

        public Func<string, string, IDataLoader<Contributor>> CreateContributorDataLoader { get; private set; }

        public IDataLoader<Contributor> GetContributorDataLoader(string userLogin, string repositoryName)
        {
            return CreateContributorDataLoader(userLogin, repositoryName);
        }
    }
}
