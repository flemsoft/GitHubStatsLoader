using Octokit;
using System;
using System.Collections.Generic;
using System.Text;

namespace GitHubTest.Models
{
    public class RepositoryStatInfo
    {
        public Repository Repository { get; set; }

        public IEnumerable<ContributorInfo> Contributors { get; set; }

        public RepositoryStatInfo(Repository repository, IEnumerable<ContributorInfo> contributors)
        {
            Repository = repository;
            Contributors = contributors;
        }
    }
}
