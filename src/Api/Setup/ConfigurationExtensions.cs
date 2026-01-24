using Azure.Identity;
using Common;

namespace Api.Setup;

internal static class ConfigurationExtensions
{
    internal static void AddConfiguration(this ConfigurationManager configManager, IWebHostEnvironment environment)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        configManager.AddConfiguration(config);

        if (environment.IsDevelopment() && EnvVarUtils.HasNoApiUrl())
        {
            var settings = new Dictionary<string, string?>
            {
                [EnvVarKeys.ApiUrl] = "http://localhost:9091"
            };
            configManager.AddInMemoryCollection(settings);
        }

        var keyVaultName = configManager.GetValue<string>(EnvVarKeys.KeyVaultName);
        if (environment.IsProduction() && !string.IsNullOrEmpty(keyVaultName))
        {
            var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");
            configManager.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());
        }
    }
}
