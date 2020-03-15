using GitHubTest.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubTest
{
    public class RepositoryInfoLoader
    {
        const int MaxPages = 3;

        private readonly IGitHubService _dataService;

        public RepositoryInfoLoader(IGitHubService dataService)
        {
            _dataService = dataService;
        }

        public async IAsyncEnumerable<Models.RepositoryStatInfo> GetRepositoryInfosAsync(string userLogin)
        {
            var repositoriesLoader = _dataService.GetRepositoryDataLoader(userLogin);
            await foreach (var repositoryPage in GetPagedEntitiesAsync(repositoriesLoader))
            {
                // We can load statistics in parallel for group of repositories
                var loadStatsTasks = repositoryPage.Select(r => LoadRepositoryStatisticsAsync(r)).ToArray();
                Task.WaitAll(loadStatsTasks);

                foreach (var task in loadStatsTasks)
                {
                    yield return task.Result;
                }
            }
        }

        private async Task<Models.RepositoryStatInfo> LoadRepositoryStatisticsAsync(Octokit.Repository repository)
        {
            if (string.IsNullOrEmpty(repository.Owner?.Login) ||
                string.IsNullOrEmpty(repository.Name))
            {
                throw new InvalidOperationException("Invalid GitHub response");
            }

            var contributors = new List<Models.ContributorInfo>();
            var contributorsLoader = _dataService.GetContributorDataLoader(repository.Owner.Login, repository.Name);
            await foreach (var contributorsPage in GetPagedEntitiesAsync(contributorsLoader))
            {
                contributors.AddRange(contributorsPage);
            }

            return new Models.RepositoryStatInfo(repository, contributors);
        }

        private static async IAsyncEnumerable<IEnumerable<T>> GetPagedEntitiesAsync<T>(IDataLoader<T> loader)
        {
            int pagesCount = 0;

            // Stop with 10 pages, because these are large list of results
            while (loader.HasMorePages && (pagesCount++ < MaxPages))
            {
                yield return await loader.LoadPage();
            }
        }
    }
}
