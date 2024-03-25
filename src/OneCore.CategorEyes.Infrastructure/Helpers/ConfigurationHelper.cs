using Microsoft.Extensions.Configuration;

namespace OneCore.CategorEyes.Infrastructure.Helpers
{
    internal static class ConfigurationHelper
    {
        private static readonly IConfigurationRoot _configuration;

        static ConfigurationHelper()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();
        }

        public static string Value(string key)
            => _configuration[key];
    }
}
