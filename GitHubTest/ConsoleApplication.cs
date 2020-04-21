using GitHubTest.Output;
using GitHubTest.RepositoryData;
using System;
using System.Threading.Tasks;

namespace GitHubTest
{
    public class ConsoleApplication
    {
        private readonly IRepositoryInfoService _dataService;
        private readonly IResultReciever _resultReciever;

        public ConsoleApplication(IRepositoryInfoService dataService, IResultReciever resultReciever)
        {
            _dataService = dataService;
            _resultReciever = resultReciever;
        }

        public async Task Run()
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Length < 2)
            {
                Console.WriteLine("User login not defined");
                return;
            }
            var userLogin = args[1];

            await foreach (var repositoryInfo in _dataService.GetRepositoryInfosAsync(userLogin))
            {
                _resultReciever.AddRepositoryStat(repositoryInfo);
            }
        }
    }
}
