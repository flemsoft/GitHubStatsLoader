using GitHubTest.Models;

namespace GitHubTest.Tests.Mocks
{
    class RepositoryMock : Repository
    {
        public RepositoryMock(string name, string ownerLogin)
        {
            Name = name;
            Owner = new UserMosk(ownerLogin);
        }

        class UserMosk : User
        {
            public UserMosk(string login)
            {
                Login = login;
            }
        }
    }
}
