namespace Common;

public static class EnvVarKeys
{
    public const string KeyVaultName = "KEY_VAULT_NAME";
    public const string UseJsonFormatting = "USE_JSON_FORMATTING";
    public const string UseOutputCache = "USE_OUTPUT_CACHE";
    public const string MarketClientUrl = "MARKET_CLIENT_URL";
    public const string UseMocking = "USE_MOCKING";
}

public static class EnvVarAccessors
{
    public static bool UseMocking =>
        bool.TryParse(Environment.GetEnvironmentVariable(EnvVarKeys.UseMocking), out var value) && value;

    public static bool UseJsonFormatting =>
        bool.TryParse(Environment.GetEnvironmentVariable(EnvVarKeys.UseJsonFormatting), out var value) && value;
}
