namespace IntegrationTests.Setup;

public static class Constants
{
    public const string BaseUrl = "/api/v1";
    public const string DateFormat = "yyyy-MM-dd";

    public static readonly DateOnly s_today = DateOnly.FromDateTime(DateTime.UtcNow);
    public static readonly DateOnly s_mockDate = new(2025, 8, 30);
}
