using Polly;
using Polly.Bulkhead;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GitHubTest.RepositoryData
{
    public class RepositoryInfoService : IRepositoryInfoService
    {
        const int MaxPages = 10;
        const int MaxActiveThreads = 20;

        // We need to limit active working threads to prevent the thread pool flooding due to the huge amount of parallel requests.
        private readonly AsyncBulkheadPolicy _bulkhead;

        private readonly IDataLoaderFactory _dataLoaderFactory;
        
        public RepositoryInfoService(IDataLoaderFactory dataLoaderFactory)
        {
            _dataLoaderFactory = dataLoaderFactory;

            _bulkhead = Policy.BulkheadAsync(MaxActiveThreads, int.MaxValue);
        }

        public async IAsyncEnumerable<Models.RepositoryStatInfo> GetRepositoryInfosAsync(string userLogin)
        {
            var repositoriesLoader = _dataLoaderFactory.GetRepositoryDataLoader(userLogin);
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
        
        private async Task<Models.RepositoryStatInfo> LoadRepositoryStatisticsAsync(Models.Repository repository)
        {
            if (string.IsNullOrEmpty(repository.Owner?.Login) ||
                string.IsNullOrEmpty(repository.Name))
            {
                throw new InvalidOperationException("Invalid GitHub response");
            }

            var contributors = new List<Models.Contributor>();
            var contributorsLoader = _dataLoaderFactory.GetContributorDataLoader(repository.Owner.Login, repository.Name);
            await foreach (var contributorsPage in GetPagedEntitiesAsync(contributorsLoader))
            {
                contributors.AddRange(contributorsPage);
            }

            return new Models.RepositoryStatInfo(repository, contributors);
        }

        private async IAsyncEnumerable<IEnumerable<T>> GetPagedEntitiesAsync<T>(IDataLoader<T> loader)
        {
            int pagesCount = 0;

            // Stop with 10 pages, because these are large list of results
            while (loader.HasMorePages && (pagesCount++ < MaxPages))
            {
                yield return await _bulkhead.ExecuteAsync(() =>
                    loader.LoadPage());
            }
        }
    }
}
