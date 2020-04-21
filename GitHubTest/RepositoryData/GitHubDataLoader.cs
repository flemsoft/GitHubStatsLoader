using Octokit;
using Polly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GitHubTest.RepositoryData
{
    public class GitHubDataLoader<TEntity> : IDataLoader<TEntity>
    {
        //const int DefaultPageSize = 30;

        const int MaxAttemptCount = 3;

        public string NextPageUri { get; set; }

        public bool HasMorePages { get { return NextPageUri != null; } }

        private readonly GitHubClient _client;

        public GitHubDataLoader(GitHubClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<TEntity>> LoadPage()
        {
            // We don't use native Octokit methods because possible problems with multithreaded calls to _client.GetLastApiInfo()
            //var options = new ApiOptions
            //{
            //    StartPage = 1,
            //    PageCount = 1,
            //    PageSize = DefaultPageSize,
            //};
            //var response = await _client.Repository.GetAllForUser(userlogin, options);
            //var apiInfo = _client.GetLastApiInfo();

            var withRetry = Policy.Handle<Exception>()
                .RetryAsync(MaxAttemptCount);

            return await withRetry.ExecuteAsync(async () =>
            {
                var response = await _client.Connection.Get<IEnumerable<TEntity>>(
                    new Uri(this.NextPageUri),
                    null, "application/json");

                this.NextPageUri = FindNextPageUri(response.HttpResponse.Headers);

                return response.Body ?? new TEntity[] { };
            });
        }

        public static string FindNextPageUri(IReadOnlyDictionary<string, string> headers)
        {
            // Trying to find the next page uri
            // TODO Parse using regular expressions
            if (headers.TryGetValue("Link", out string linkHeader))
            {
                foreach (var linkPart in linkHeader.Split(','))
                {
                    var parts = linkPart.Split(';');
                    if (parts.Length < 2)
                        continue;

                    if (parts[1].Contains("next"))
                    {
                        return parts[0].Trim(' ', '<', '>');
                    }
                }
            }

            return null;
        }
    }
}
