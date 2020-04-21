using GitHubTest.Models;

namespace GitHubTest.RepositoryData
{
    public interface IDataLoaderFactory
    {
        IDataLoader<Repository> GetRepositoryDataLoader(string userLogin);

        IDataLoader<Contributor> GetContributorDataLoader(string userLogin, string repositoryName);
    }
}
