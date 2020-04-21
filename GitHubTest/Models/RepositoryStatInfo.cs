using System.Collections.Generic;

namespace GitHubTest.Models
{
    public class RepositoryStatInfo
    {
        public Repository Repository { get; set; }

        public IEnumerable<Contributor> Contributors { get; set; }

        public RepositoryStatInfo(Repository repository, IEnumerable<Contributor> contributors)
        {
            Repository = repository;
            Contributors = contributors;
        }
    }
}
