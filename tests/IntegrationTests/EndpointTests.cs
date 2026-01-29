using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using Api.Endpoints.Bitcoin;
using Common;
using IntegrationTests.Setup;
using Shouldly;
using Xunit;

namespace IntegrationTests;

public sealed class EndpointsTests(WiremockFixture fixture, ITestOutputHelper outputHelper) : TestBase(fixture, outputHelper)
{
    public static TheoryData<string, string> OkCases =>
    new()
    {
        {
            Constants.StartMockDate.ToString(Constants.DateFormat, CultureInfo.InvariantCulture),
            Constants.EndMockDate.ToString(Constants.DateFormat, CultureInfo.InvariantCulture)
        },
    };

    [Theory]
    [MemberData(nameof(OkCases))]
    public async Task LongestDownwardTrendReturnsOK(string fromDate, string toDate)
    {
        var url = new Uri($"{Constants.BaseUrl}/longestdownwardtrend?fromDate={fromDate}&toDate={toDate}", UriKind.Relative);

        var ct = TestContext.Current.CancellationToken;
        var result = await _client.GetAsync(url, cancellationToken: ct);

        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        var data = await result.Content.ReadFromJsonAsync<LongestDownwardTrendResponse>(ct);
        data.ShouldNotBeNull();
        data.Days.ShouldBe(3);
    }

    [Theory]
    [MemberData(nameof(OkCases))]
    public async Task HighestTradingVolumeReturnsOK(string fromDate, string toDate)
    {
        var url = new Uri($"{Constants.BaseUrl}/highesttradingvolume?fromDate={fromDate}&toDate={toDate}", UriKind.Relative);

        var ct = TestContext.Current.CancellationToken;
        var result = await _client.GetAsync(url, cancellationToken: ct);

        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        var data = await result.Content.ReadFromJsonAsync<HighestTradingVolumeResponse>(ct);
        data.ShouldNotBeNull();
        data.Date.ShouldBe(new DateOnly(2025, 9, 6));
        data.Volume.ShouldBe(48013311912.10m, 0.01m);
    }

    [Theory]
    [MemberData(nameof(OkCases))]
    public async Task BuyAndSellReturnsOK(string fromDate, string toDate)
    {
        var url = new Uri($"{Constants.BaseUrl}/buyandsell?fromDate={fromDate}&toDate={toDate}", UriKind.Relative);

        var ct = TestContext.Current.CancellationToken;
        var result = await _client.GetAsync(url, cancellationToken: ct);

        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        var data = await result.Content.ReadFromJsonAsync<BuyAndSellResponse>(ct);
        data.ShouldNotBeNull();
        data.BuyDate.ShouldBe(new DateOnly(2025, 9, 1));
        data.SellDate.ShouldBe(new DateOnly(2025, 9, 4));

    }

    [Theory]
    [InlineData("longestdownwardtrend")]
    [InlineData("highesttradingvolume")]
    [InlineData("buyandsell")]
    public async Task EndpointsReturnNoContent(string endpoint)
    {
        var fromDate = Constants.Today.ToString(Constants.DateFormat, CultureInfo.InvariantCulture);
        var toDate = Constants.Today.AddMonths(1).ToString(Constants.DateFormat, CultureInfo.InvariantCulture);
        var url = new Uri($"{Constants.BaseUrl}/{endpoint}?fromDate={fromDate}&toDate={toDate}", UriKind.Relative);

        var ct = TestContext.Current.CancellationToken;
        var result = await _client.GetAsync(url, cancellationToken: ct);

        result.StatusCode.ShouldBeOneOf(HttpStatusCode.NoContent);
    }

    [Theory]
    [InlineData("longestdownwardtrend")]
    [InlineData("highesttradingvolume")]
    [InlineData("buyandsell")]
    public async Task EndpointsReturnUnauthorized(string endpoint)
    {
        var fromDate = Constants.StartMockDate.AddYears(-1).AddDays(-1).ToString(Constants.DateFormat, CultureInfo.InvariantCulture);
        var toDate = Constants.StartMockDate.ToString(Constants.DateFormat, CultureInfo.InvariantCulture);
        var url = new Uri($"{Constants.BaseUrl}/{endpoint}?fromDate={fromDate}&toDate={toDate}", UriKind.Relative);

        var ct = TestContext.Current.CancellationToken;
        var result = await _client.GetAsync(url, cancellationToken: ct);

        result.StatusCode.ShouldBeOneOf(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData("longestdownwardtrend")]
    [InlineData("highesttradingvolume")]
    [InlineData("buyandsell")]
    public async Task EndpointsReturnBadRequest(string endpoint)
    {
        var fromDate = string.Empty;
        string? toDate = null;
        var url = new Uri($"{Constants.BaseUrl}/{endpoint}?fromDate={fromDate}&toDate={toDate}", UriKind.Relative);

        var ct = TestContext.Current.CancellationToken;
        var result = await _client.GetAsync(url, cancellationToken: ct);

        result.StatusCode.ShouldBeOneOf(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Health()
    {
        var ct = TestContext.Current.CancellationToken;
        var result = await _client.GetAsync(new Uri("/health", UriKind.Relative), ct);

        result.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
