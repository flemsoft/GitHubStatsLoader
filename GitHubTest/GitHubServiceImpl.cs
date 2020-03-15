using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GitHubTest.Interfaces;
using Octokit;

namespace GitHubTest
{
    public class GitHubServiceImpl : IGitHubService
    {
        private readonly GitHubClient _client;

        public GitHubServiceImpl(string gitHubKey)
        {
            if (string.IsNullOrEmpty(gitHubKey))
                throw new ArgumentException("Invalid GitHub token");

            _client = new GitHubClient(new ProductHeaderValue("GitHubTest"))
            {
                Credentials = new Credentials(gitHubKey)
            };
        }

        public IDataLoader<Repository> GetRepositoryDataLoader(string userLogin)
        {
            return new GitHubDataLoader<Repository>(_client)
            {
                NextPageUri = $"https://api.github.com/users/{userLogin}/repos",
            };
        }

        public IDataLoader<Models.ContributorInfo> GetContributorDataLoader(string userLogin, string repositoryName)
        {
            return new GitHubDataLoader<Models.ContributorInfo>(_client)
            {
                NextPageUri = $"https://api.github.com/repos/{userLogin}/{repositoryName}/stats/contributors",
            };
        }
    }
}
