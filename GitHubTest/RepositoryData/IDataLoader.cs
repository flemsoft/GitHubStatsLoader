using System.Collections.Generic;
using System.Threading.Tasks;

namespace GitHubTest.RepositoryData
{
    public interface IDataLoader<TEntity>
    {
        bool HasMorePages { get; }

        Task<IEnumerable<TEntity>> LoadPage();
    }
}