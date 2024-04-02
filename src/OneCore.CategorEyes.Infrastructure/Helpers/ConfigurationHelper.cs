using Microsoft.Extensions.Configuration;

namespace OneCore.CategorEyes.Infrastructure.Helpers
{
    internal static class ConfigurationHelper
    {
        private static readonly IConfigurationRoot _configuration;

        /// <summary>
        /// Initializes the configuration by reading values from the "appsettings.json" and "appsettings.Development.json" files, as well as from environment variables.
        /// This constructor is automatically executed the first time the ConfigurationHelper class is referenced, due to its static nature.
        /// </summary>
        static ConfigurationHelper()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();
        }

        /// <summary>
        /// Retrieves the configuration value associated with the specified key.
        /// </summary>
        /// <param name="key">The configuration key whose value is to be retrieved, of type <see cref="string"/>.</param>
        /// <returns>The configuration value as a <see cref="string"/>. Returns <c>null</c> if the key does not exist.</returns>
        public static string Value(string key)
            => _configuration[key];
    }
}
