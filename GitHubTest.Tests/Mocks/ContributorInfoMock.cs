using GitHubTest.Models;

namespace GitHubTest.Tests.Mocks
{
    class ContributorInfoMock : Contributor
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
