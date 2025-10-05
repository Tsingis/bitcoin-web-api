using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using Api.Endpoints;
using IntegrationTests.Setup;
using Shouldly;
using Xunit;

namespace IntegrationTests;

public sealed class ApiEndpointsTests(Fixture fixture) : IntegrationTestBase(fixture)
{
    private const string DateFormat = "yyyy-MM-dd";
    private const string BaseUrl = "/api/v1";

    private static readonly DateOnly s_today = DateOnly.FromDateTime(DateTime.UtcNow);
    private static readonly DateOnly s_mockCutOffDate = new(2024, 8, 29);

    public static TheoryData<string?, string?, HttpStatusCode> Cases =>
    new()
    {
        {s_today.AddMonths(-1).ToString(DateFormat, CultureInfo.InvariantCulture), s_today.ToString(DateFormat, CultureInfo.InvariantCulture), HttpStatusCode.OK},
        {s_mockCutOffDate.ToString(DateFormat, CultureInfo.InvariantCulture), s_mockCutOffDate.AddYears(1).AddDays(1).ToString(DateFormat, CultureInfo.InvariantCulture), HttpStatusCode.Unauthorized}, //Unauthorized for over 365 days old queries
    };

    [Theory]
    [MemberData(nameof(Cases))]
    public async Task LongestDownwardTrend(string? fromDate, string? toDate, HttpStatusCode status)
    {
        var ct = TestContext.Current.CancellationToken;
        var result = await _client.GetAsync(new Uri($"{BaseUrl}/longestdownwardtrend?fromDate={fromDate}&toDate={toDate}", UriKind.Relative), cancellationToken: ct);
        result.StatusCode.ShouldBe(status);

        if (result.StatusCode == HttpStatusCode.OK)
        {
            var data = await result.Content.ReadFromJsonAsync<LongestDownwardTrendResponse>(cancellationToken: ct);
            data.ShouldNotBeNull();
        }
    }
}
