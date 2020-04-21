using GitHubTest.Output;
using GitHubTest.RepositoryData;
using GitHubTest.Settings;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace GitHubTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            var services = ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();
            await serviceProvider.GetService<ConsoleApplication>().Run();
        }

        private static IServiceCollection ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddTransient<ISettingsProvider, SettingsProviderService>();
            services.AddTransient<IResultReciever, ResultReciever>();
            services.AddTransient<IDataLoaderFactory, DataLoaderFactoryImpl>();
            services.AddTransient<IRepositoryInfoService, RepositoryInfoService>();
            // IMPORTANT! Register our application entry point
            services.AddTransient<ConsoleApplication>();
            return services;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine("Unhandled exception. " + e.ExceptionObject);
        }
    }
}
