﻿using GitHubTest.Interfaces;
using Octokit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GitHubTest
{
    public class GitHubDataLoader<TEntity> : IDataLoader<TEntity>
    {
        //const int DefaultPageSuze = 30;

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
            //    PageSize = DefaultPageSuze,
            //};
            //var response = await _client.Repository.GetAllForUser(userlogin, options);
            //var apiInfo = _client.GetLastApiInfo();

            var exceptions = new List<Exception>();

            for (var attemptCount = 0; attemptCount < MaxAttemptCount; attemptCount++)
            {
                try
                {
                    var response = await _client.Connection.Get<IEnumerable<TEntity>>(
                        new Uri(this.NextPageUri),
                        null, "application/json");

                    this.NextPageUri = FindNextPageUri(response.HttpResponse.Headers);

                    return response.Body ?? new TEntity[] { };
                }
                catch (Exception ex)
                {
                    // TODO Add errors logs
                    exceptions.Add(ex);
                }
            }

            throw new AggregateException(exceptions);
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
