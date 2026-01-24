namespace Common;

public static class EnvVarKeys
{
    public const string KeyVaultName = "KEY_VAULT_NAME";
    public const string UseJsonFormatting = "USE_JSON_FORMATTING";
    public const string UseOutputCache = "USE_OUTPUT_CACHE";
    public const string UseRateLimiter = "USE_RATE_LIMITER";
    public const string ApiUrl = "API_URL";
    public const string ShowMockServerLogs = "SHOW_MOCK_SERVER_LOGS";
    public const string TestingLogLevel = "TESTING_LOG_LEVEL";
}

public static class EnvVarUtils
{
    public static bool UseMockServer => HasNoApiUrl();

    public static bool ShowMockServerLogs =>
        bool.TryParse(Environment.GetEnvironmentVariable(EnvVarKeys.ShowMockServerLogs), out var value) && value;

    public static bool UseJsonFormatting =>
        bool.TryParse(Environment.GetEnvironmentVariable(EnvVarKeys.UseJsonFormatting), out var value) && value;

    public static bool HasNoApiUrl()
    {
        var url = Environment.GetEnvironmentVariable(EnvVarKeys.ApiUrl);
        return string.IsNullOrEmpty(url)
            || url.Contains("localhost")
            || url.Contains("wiremock");
    }
}
