using GitHubTest.Models;

namespace GitHubTest.Output
{
    public interface IResultReciever
    {
        void AddRepositoryStat(RepositoryStatInfo info);
    }
}
