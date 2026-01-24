namespace Common;

public static class Constants
{
    public const string BaseUrl = "/api/v1/bitcoin";
    public const string DateFormat = "yyyy-MM-dd";

    public static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.UtcNow);
    public static readonly DateOnly StartMockDate = new(2025, 8, 30);
    public static readonly DateOnly EndMockDate = new(2025, 9, 10);
}
