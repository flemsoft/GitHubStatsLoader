using Octokit;
using System;
using System.Collections.Generic;
using System.Text;

namespace GitHubTest.Models
{
    public class ContributorInfo
    {
        public Author Author { get; protected set; }

        public int Total { get; protected set; }

        //public IReadOnlyList<WeeklyHash> Weeks { get; protected set; }
    }
}
