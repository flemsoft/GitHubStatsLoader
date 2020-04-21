using GitHubTest.Models;
using System;
using System.Linq;

namespace GitHubTest.Output
{
    public class ResultReciever : IResultReciever
    {
        public void AddRepositoryStat(RepositoryStatInfo info)
        {
            Console.WriteLine(info.Repository.Name + ":");

            if (!info.Contributors.Any())
            {
                Console.WriteLine($"\tNo contributors");
                return;
            }

            foreach (var contributor in info.Contributors.OrderByDescending(x => x.Total))
            {
                Console.WriteLine($"\t{contributor.Author.Login} - {contributor.Total}");
            }
        }
    }
}
