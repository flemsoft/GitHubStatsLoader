using Octokit;
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

            if (string.IsNullOrEmpty(GitHubKey))
                throw new ArgumentException("Invalid GitHub token");

            var client = new GitHubClient(new ProductHeaderValue("GitHubTest"))
            {
                Credentials = new Credentials(GitHubKey)
            };

            var firstPageRepositoryQuery = $"https://api.github.com/users/{args[0]}/repos";
            await foreach (var repositoryPage in GetPagedEntitiesAsync<Repository>(client, firstPageRepositoryQuery))
            {
                // We can load statistics in parallel for group of repositories
                var loadStatsTasks = repositoryPage.Select(r => LoadRepositoryStatisticsAsync(client, r)).ToArray();
                Task.WaitAll(loadStatsTasks);

                foreach (var task in loadStatsTasks)
                {
                    Console.WriteLine(task.Result.Item1.Name + ":");

                    if (!task.Result.Item2.Any())
                    {
                        Console.WriteLine($"\tNo contributors");
                        continue;
                    }

                    foreach (var info in task.Result.Item2.OrderByDescending(x => x.Total))
                    {
                        Console.WriteLine($"\t{info.Author.Login} - {info.Total}");
                    }
                }
            }
        }

        private static async IAsyncEnumerable<IEnumerable<T>> GetPagedEntitiesAsync<T>(GitHubClient client, string query, int maxPages = 10)
        {
            bool hasMorePages = true;
            int pagesCount = 0;

            // Stop with 10 pages, because these are large list of results
            while (hasMorePages && (pagesCount++ < maxPages))
            {
                var response = await client.Connection.Get<IEnumerable<T>>(
                    new Uri(query),
                    null, "application/json");

                // Trying to find the next page uri
                // TODO Parse using regular expressions
                string nextPageUri = null;
                if (response.HttpResponse.Headers.TryGetValue("Link", out string linkHeader))
                {
                    foreach (var linkPart in linkHeader.Split(','))
                    {
                        var parts = linkPart.Split(';');
                        if (parts.Length < 2)
                            continue;

                        if (parts[1].Contains("next"))
                        {
                            nextPageUri = parts[0].Trim(' ', '<', '>');
                            break;
                        }
                    }
                }

                hasMorePages = !string.IsNullOrEmpty(nextPageUri);
                query = nextPageUri;

                yield return response.Body ?? new T[] { };
            }
        }

        private static async Task<Tuple<Repository, IEnumerable<ContributorInfo>>> LoadRepositoryStatisticsAsync(GitHubClient client, Repository repository)
        {
            if (string.IsNullOrEmpty(repository.Owner.Login) ||
                string.IsNullOrEmpty(repository.Name))
            {
                throw new InvalidOperationException("Invalid GitHub response");
            }

            var contributors = new List<ContributorInfo>();
            var firstPageContributorsQuery = $"https://api.github.com/repos/{repository.Owner.Login}/{repository.Name}/stats/contributors";
            await foreach (var contributorsPage in GetPagedEntitiesAsync<ContributorInfo>(client, firstPageContributorsQuery))
            {
                contributors.AddRange(contributorsPage);
            }

            return Tuple.Create<Repository, IEnumerable<ContributorInfo>>(repository, contributors);
        }

        public class ContributorInfo
        {
            public Author Author { get; protected set; }

            public int Total { get; protected set; }

            //public IReadOnlyList<WeeklyHash> Weeks { get; protected set; }
        }
    }
}
