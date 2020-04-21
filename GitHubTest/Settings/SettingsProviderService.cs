using System;

namespace GitHubTest.Settings
{
    public class SettingsProviderService : ISettingsProvider
    {
        // TODO Put your default GitHub token here
        const string DefaultGitHubKey = "";

        public string GitHubKey
        {
            get
            {
                var value = GetEnvironmentVariable("GitHubKey", DefaultGitHubKey);
                return value;
            }
        }

        private static string GetEnvironmentVariable(string variable, string defaultValue = null)
        {
            var value = Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.User);

            if (string.IsNullOrWhiteSpace(value))
            {
                if (!string.IsNullOrWhiteSpace(defaultValue))
                {
                    return defaultValue;
                }

                Console.WriteLine($"Environment variable not found: " + variable);
                Environment.Exit(0);
            }

            return value;
        }
    }
}
