using GitHubTest.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace GitHubTest
{
    class Program
    {
        // TODO Put your GitHub token here
        const string GitHubKey = "";

        static async Task Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("User login not defined");
                return;
            }

            IGitHubService dataService = new GitHubServiceImpl(GitHubKey);

            var loader = new RepositoryInfoLoader(dataService);

            await foreach (var repositoryInfo in loader.GetRepositoryInfosAsync(args[0]))
            {
                Console.WriteLine(repositoryInfo.Repository.Name + ":");

                if (!repositoryInfo.Contributors.Any())
                {
                    Console.WriteLine($"\tNo contributors");
                    continue;
                }

                foreach (var info in repositoryInfo.Contributors.OrderByDescending(x => x.Total))
                {
                    Console.WriteLine($"\t{info.Author.Login} - {info.Total}");
                }
            }
        }
    }
}
