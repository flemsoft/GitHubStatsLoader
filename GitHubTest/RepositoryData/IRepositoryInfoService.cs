using GitHubTest.Models;
using System.Collections.Generic;

namespace GitHubTest.RepositoryData
{
    public interface IRepositoryInfoService
    {
        IAsyncEnumerable<RepositoryStatInfo> GetRepositoryInfosAsync(string userLogin);
    }
}