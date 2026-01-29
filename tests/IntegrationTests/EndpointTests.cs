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
    [Fact]
    public async Task LongestDownwardTrend_OK()
    {
        var ct = TestContext.Current.CancellationToken;

        var url = Utility.BuildUri("longestdownwardtrend", Constants.StartMockDate, Constants.EndMockDate);
        var result = await _client.GetAsync(url, cancellationToken: ct);

        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        var data = await result.Content.ReadFromJsonAsync<LongestDownwardTrendResponse>(ct);
        data.ShouldNotBeNull();
        data.Days.ShouldBe(3);
    }

    [Fact]
    public async Task HighestTradingVolume_OK()
    {
        var ct = TestContext.Current.CancellationToken;

        var url = Utility.BuildUri("highesttradingvolume", Constants.StartMockDate, Constants.EndMockDate);
        var result = await _client.GetAsync(url, cancellationToken: ct);

        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        var data = await result.Content.ReadFromJsonAsync<HighestTradingVolumeResponse>(ct);

        data.ShouldNotBeNull();
        data.Date.ShouldBe(new DateOnly(2025, 9, 6));
        data.Volume.ShouldBe(48013311912.10m, 0.01m);
    }

    [Fact]
    public async Task BuyAndSell_OK()
    {
        var ct = TestContext.Current.CancellationToken;

        var url = Utility.BuildUri("buyandsell", Constants.StartMockDate, Constants.EndMockDate);
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
    public async Task Endpoints_NoContent(string endpoint)
    {
        var ct = TestContext.Current.CancellationToken;

        var fromDate = Constants.Today;
        var toDate = Constants.Today.AddMonths(1);
        var url = Utility.BuildUri(endpoint, fromDate, toDate);

        var result = await _client.GetAsync(url, cancellationToken: ct);

        result.StatusCode.ShouldBeOneOf(HttpStatusCode.NoContent);
    }

    [Theory]
    [InlineData("longestdownwardtrend")]
    [InlineData("highesttradingvolume")]
    [InlineData("buyandsell")]
    public async Task Endpoints_Unauthorized(string endpoint)
    {
        var ct = TestContext.Current.CancellationToken;

        var fromDate = Constants.StartMockDate.AddYears(-1).AddDays(-1);
        var toDate = Constants.StartMockDate;
        var url = Utility.BuildUri(endpoint, fromDate, toDate);

        var result = await _client.GetAsync(url, cancellationToken: ct);

        result.StatusCode.ShouldBeOneOf(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData("longestdownwardtrend")]
    [InlineData("highesttradingvolume")]
    [InlineData("buyandsell")]
    public async Task Endpoints_BadRequest(string endpoint)
    {
        var ct = TestContext.Current.CancellationToken;
        var url = Utility.BuildUri(endpoint, string.Empty, null);

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
