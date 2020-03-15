using Octokit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GitHubTest.Interfaces
{
    public interface IGitHubService
    {
        IDataLoader<Repository> GetRepositoryDataLoader(string userLogin);

        IDataLoader<Models.ContributorInfo> GetContributorDataLoader(string userLogin, string repositoryName);
    }
}
