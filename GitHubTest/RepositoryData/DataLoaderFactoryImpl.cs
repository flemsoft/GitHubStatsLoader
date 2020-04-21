using GitHubTest.Settings;
using Octokit;
using System;

namespace GitHubTest.RepositoryData
{
    public class DataLoaderFactoryImpl : IDataLoaderFactory
    {
        private readonly GitHubClient _client;

        public DataLoaderFactoryImpl(ISettingsProvider settingsProvider)
        {
            if (string.IsNullOrEmpty(settingsProvider.GitHubKey))
                throw new ArgumentException("Invalid GitHub token");

            _client = new GitHubClient(new ProductHeaderValue("GitHubTest"))
            {
                Credentials = new Credentials(settingsProvider.GitHubKey)
            };
        }

        public IDataLoader<Models.Repository> GetRepositoryDataLoader(string userLogin)
        {
            return new GitHubDataLoader<Models.Repository>(_client)
            {
                NextPageUri = $"https://api.github.com/users/{userLogin}/repos",
            };
        }

        public IDataLoader<Models.Contributor> GetContributorDataLoader(string userLogin, string repositoryName)
        {
            return new GitHubDataLoader<Models.Contributor>(_client)
            {
                NextPageUri = $"https://api.github.com/repos/{userLogin}/{repositoryName}/stats/contributors",
            };
        }
    }
}
