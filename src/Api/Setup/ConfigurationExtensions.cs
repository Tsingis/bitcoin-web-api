using Azure.Identity;
using Common;

namespace Api.Setup;

public static class ConfigurationExtensions
{
    public static void AddConfiguration(this ConfigurationManager configManager, IWebHostEnvironment environment)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        configManager.AddConfiguration(config);

        var keyVaultName = configManager.GetValue<string>(EnvVarKeys.KeyVaultName);
        if (environment.IsProduction() && !string.IsNullOrEmpty(keyVaultName))
        {
            var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");
            configManager.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());
        }
    }
}
