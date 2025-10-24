namespace Common;

public static class EnvVarKeys
{
    public const string KeyVaultName = "KEY_VAULT_NAME";
    public const string UseJsonFormatting = "USE_JSON_FORMATTING";
    public const string UseOutputCache = "USE_OUTPUT_CACHE";
    public const string MarketClientUrl = "MARKET_CLIENT_URL";
    public const string UseMockServer = "USE_MOCK_SERVER";
    public const string ShowMockServerLogs = "SHOW_MOCK_SERVER_LOGS";
    public const string TestingLogLevel = "TESTING_LOG_LEVEL";
}

public static class EnvVarAccessors
{
    public static bool UseMockServer =>
        bool.TryParse(Environment.GetEnvironmentVariable(EnvVarKeys.UseMockServer), out var value) && value;

    public static bool ShowMockServerLogs =>
        bool.TryParse(Environment.GetEnvironmentVariable(EnvVarKeys.ShowMockServerLogs), out var value) && value;

    public static bool UseJsonFormatting =>
        bool.TryParse(Environment.GetEnvironmentVariable(EnvVarKeys.UseJsonFormatting), out var value) && value;
}
