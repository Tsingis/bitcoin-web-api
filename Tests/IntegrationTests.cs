using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

public class IntegrationTests(IntegrationFixture fixture) : IClassFixture<IntegrationFixture>
{
    private const string BaseUrl = "/api/v1";
    private readonly IntegrationFixture _fixture = fixture;

    [Theory]
    [InlineData("2022-01-01", "2022-01-31", HttpStatusCode.OK)]
    [InlineData("1822-01-01", "1822-01-31", HttpStatusCode.NotFound)]
    [InlineData("", null, HttpStatusCode.BadRequest)]
    public async Task LongestDownwardTrend(string? fromDate, string? toDate, HttpStatusCode status)
    {
        var result = await _fixture.Client.GetAsync($"{BaseUrl}/longestdownwardtrend?fromDate={fromDate}&toDate={toDate}");
        result.StatusCode.Should().BeOneOf(status, HttpStatusCode.TooManyRequests);
    }

    [Theory]
    [InlineData("2022-01-01", "2022-01-31", HttpStatusCode.OK)]
    [InlineData("1822-01-01", "1822-01-31", HttpStatusCode.NotFound)]
    [InlineData("", null, HttpStatusCode.BadRequest)]
    public async Task HighestTradingVolume(string? fromDate, string? toDate, HttpStatusCode status)
    {
        var result = await _fixture.Client.GetAsync($"{BaseUrl}/highestradingvolume?fromDate={fromDate}&toDate={toDate}");
        result.StatusCode.Should().BeOneOf(status, HttpStatusCode.TooManyRequests);
    }

    [Theory]
    [InlineData("2022-01-01", "2022-01-31", HttpStatusCode.OK)]
    [InlineData("1822-01-01", "1822-01-31", HttpStatusCode.NotFound)]
    [InlineData("", null, HttpStatusCode.BadRequest)]
    public async Task BuyAndSell(string? fromDate, string? toDate, HttpStatusCode status)
    {
        var result = await _fixture.Client.GetAsync($"{BaseUrl}/buyandsell?fromDate={fromDate}&toDate={toDate}");
        result.StatusCode.Should().BeOneOf(status, HttpStatusCode.TooManyRequests);
    }

    [Fact]
    public async Task Swagger()
    {
        var result = await _fixture.Client.GetAsync("/swagger");
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Health()
    {
        var result = await _fixture.Client.GetAsync("/health");
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}

