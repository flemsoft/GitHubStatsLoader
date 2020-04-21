
namespace GitHubTest.Models
{
    public class Contributor
    {
        public Author Author { get; set; }

        public int Total { get; set; }

        //public IReadOnlyList<WeeklyHash> Weeks { get; protected set; }
    }
}
