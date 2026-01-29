using System.Globalization;
using Common;

namespace IntegrationTests.Setup;

internal static class Utility
{
    internal static Uri BuildUri(string endpoint, DateOnly fromDate, DateOnly toDate)
    {
        var fromDateStr = fromDate.ToString(Constants.DateFormat, CultureInfo.InvariantCulture);
        var toDateStr = toDate.ToString(Constants.DateFormat, CultureInfo.InvariantCulture);
        return new Uri($"{Constants.BaseUrl}/{endpoint}?fromDate={fromDateStr}&toDate={toDateStr}", UriKind.Relative);
    }

    internal static Uri BuildUri(string endpoint, string? fromDateStr, string? toDateStr)
    {
        return new Uri($"{Constants.BaseUrl}/{endpoint}?fromDate={fromDateStr}&toDate={toDateStr}", UriKind.Relative);
    }
}
