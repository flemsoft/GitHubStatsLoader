using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GitHubTest.Interfaces
{
    public interface IDataLoader<TEntity>
    {
        bool HasMorePages { get; }

        Task<IEnumerable<TEntity>> LoadPage();
    }
}