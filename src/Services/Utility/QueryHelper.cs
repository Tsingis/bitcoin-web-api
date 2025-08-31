using System.Globalization;
using Common.Extensions;

namespace Services.Utility;

public static class QueryHelper
{
    public static Dictionary<string, string?> CreateQueryParams(DateOnly fromDate, DateOnly toDate, string currency = "eur")
    {
        var from = fromDate.ToUnixTimestamp().ToString(CultureInfo.InvariantCulture);
        var to = toDate.ToUnixTimestamp().ToString(CultureInfo.InvariantCulture);
        var parameters = new Dictionary<string, string?>
    {
        { "vs_currency", currency },
        { "from", from },
        { "to", to }
    };
        return parameters;
    }
}
