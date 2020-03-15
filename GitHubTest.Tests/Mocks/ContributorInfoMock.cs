using GitHubTest.Models;
using Octokit;
using System;
using System.Collections.Generic;
using System.Text;

namespace GitHubTest.Tests.Mocks
{
    class ContributorInfoMock : ContributorInfo
    {
        public ContributorInfoMock(string authorLogin, int total)
        {
            Author = new AuthorMock(authorLogin);
            Total = total;
        }

        class AuthorMock : Author
        {
            public AuthorMock(string login)
            {
                Login = login;
            }
        }
    }
}
