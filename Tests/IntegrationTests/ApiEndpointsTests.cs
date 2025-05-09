using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Api.Setup;
using Shouldly;
using Xunit;

namespace IntegrationTests;

public sealed class ApiEndpointsTests(ApplicationFactory factory) : IntegrationTestBase(factory)
{
    private const string BaseUrl = "/api/v1";

    public static TheoryData<string?, string?, HttpStatusCode> Cases =>
    new TheoryData<string?, string?, HttpStatusCode>
    {
        {DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), HttpStatusCode.OK},
        {DateTime.Now.AddMonths(-13).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), DateTime.Now.AddMonths(-12).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), HttpStatusCode.Unauthorized}, //Unauthorized for over 365 days old queries
        {"", null, HttpStatusCode.BadRequest},
    };

    [Theory]
    [MemberData(nameof(Cases))]
    public async Task LongestDownwardTrend(string? fromDate, string? toDate, HttpStatusCode status)
    {
        _factory.SetOutputHelper(TestContext.Current.TestOutputHelper);
        var ct = TestContext.Current.CancellationToken;
        var result = await _client.GetAsync($"{BaseUrl}/longestdownwardtrend?fromDate={fromDate}&toDate={toDate}", cancellationToken: ct);
        result.StatusCode.ShouldBeOneOf(status, HttpStatusCode.TooManyRequests);

        if (result.StatusCode == HttpStatusCode.OK)
        {
            var data = await result.Content.ReadFromJsonAsync<LongestDownwardTrendResponse>(cancellationToken: ct);
            data.ShouldNotBeNull();
        }
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public async Task HighestTradingVolume(string? fromDate, string? toDate, HttpStatusCode status)
    {
        var ct = TestContext.Current.CancellationToken;
        var result = await _client.GetAsync($"{BaseUrl}/highestradingvolume?fromDate={fromDate}&toDate={toDate}", cancellationToken: ct);
        result.StatusCode.ShouldBeOneOf(status, HttpStatusCode.TooManyRequests);

        if (result.StatusCode == HttpStatusCode.OK)
        {
            var data = await result.Content.ReadFromJsonAsync<HighestTradingVolumeResponse>(cancellationToken: ct);
            data.ShouldNotBeNull();
        }
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public async Task BuyAndSell(string? fromDate, string? toDate, HttpStatusCode status)
    {
        var ct = TestContext.Current.CancellationToken;
        var result = await _client.GetAsync($"{BaseUrl}/buyandsell?fromDate={fromDate}&toDate={toDate}", cancellationToken: ct);
        result.StatusCode.ShouldBeOneOf(status, HttpStatusCode.TooManyRequests);

        if (result.StatusCode == HttpStatusCode.OK)
        {
            var data = await result.Content.ReadFromJsonAsync<BuyAndSellResponse>(cancellationToken: ct);
            data.ShouldNotBeNull();
        }
    }

    [Theory]
    [InlineData("/swagger")]
    [InlineData("/scalar")]
    public async Task DocsUI(string endpoint)
    {
        var ct = TestContext.Current.CancellationToken;
        var result = await _client.GetAsync(endpoint, cancellationToken: ct);
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("/openapi/v1.json")]
    public async Task DocsJson(string endpoint)
    {
        var ct = TestContext.Current.CancellationToken;
        var result = await _client.GetAsync(endpoint, cancellationToken: ct);
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        var data = await result.Content.ReadAsStringAsync(cancellationToken: ct);

        Should.NotThrow(() =>
        {
            using var document = JsonDocument.Parse(data);
            var root = document.RootElement;

            root.TryGetProperty("openapi", out var _).ShouldBeTrue("Missing 'openapi' key");

            var firstPath = root.GetProperty("paths").EnumerateObject().First();
            var parameters = firstPath.Value
                .GetProperty("get")
                .GetProperty("parameters");

            foreach (var param in parameters.EnumerateArray())
            {
                string? example = null;

                if (param.TryGetProperty("example", out var exampleElement))
                {
                    example = exampleElement.GetString();
                }
                else if (param.TryGetProperty("schema", out var schema) &&
                         schema.TryGetProperty("example", out var schemaExample))
                {
                    example = schemaExample.GetString();
                }

                example.ShouldNotBeNullOrWhiteSpace("Missing example for parameter");
            }
        });
    }

    [Fact]
    public async Task Health()
    {
        var ct = TestContext.Current.CancellationToken;
        var result = await _client.GetAsync("/health", cancellationToken: ct);
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
