using GitHubTest.RepositoryData;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GitHubTest.Tests.Mocks
{
    class PageLoaderMock<TModel> : IDataLoader<TModel>
    {
        public IEnumerable<TModel> Models { get; private set; }

        public int PageSize { get; set; } = 5;

        public int CurrentPage { get; private set; } = 0;

        public bool HasMorePages
        {
            get
            {
                return CurrentPage * PageSize < Models.Count();
            }
        }

        public PageLoaderMock(IEnumerable<TModel> models)
        {
            Models = models;
        }

        public Task<IEnumerable<TModel>> LoadPage()
        {
            var result = Models.Skip(CurrentPage * PageSize).Take(PageSize);
            CurrentPage++;

            return Task.FromResult(result);
        }
    }
}
