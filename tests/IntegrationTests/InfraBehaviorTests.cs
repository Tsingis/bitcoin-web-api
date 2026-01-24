using System.Net;
using Common;
using IntegrationTests.Setup;
using Shouldly;
using Xunit;

namespace IntegrationTests;

public sealed class InfraBehaviorTests(WiremockFixture fixture)
{
    [Theory]
    [InlineData(false, false, false)]
    [InlineData(false, true, false)]
    [InlineData(true, false, true)]
    [InlineData(true, true, true)]
    public async Task Hits_Cache(bool useOutputCache, bool useRateLimiter, bool expectedCacheHit)
    {
        using var factory = new TestFactory(fixture, useOutputCache, useRateLimiter);
        var client = factory.CreateClient();

        var ct = TestContext.Current.CancellationToken;
        var url = new Uri($"{Constants.BaseUrl}/buyandsell?fromDate={Constants.StartMockDate}&toDate={Constants.EndMockDate}", UriKind.Relative);
        var first = await client.GetAsync(url, cancellationToken: ct);
        var second = await client.GetAsync(url, cancellationToken: ct);

        first.Headers.Select(x => x.Key).ShouldNotContain("Age");
        second.Headers.Select(x => x.Key).Contains("Age").ShouldBe(expectedCacheHit);
    }

    [Theory]
    [InlineData(false, false, HttpStatusCode.OK)]
    [InlineData(false, true, HttpStatusCode.TooManyRequests)]
    [InlineData(true, false, HttpStatusCode.OK)]
    [InlineData(true, true, HttpStatusCode.OK)]
    public async Task Hits_RateLimit(bool useOutputCache, bool useRateLimiter, HttpStatusCode expectedStatusCode)
    {
        using var factory = new TestFactory(fixture, useOutputCache, useRateLimiter);
        var client = factory.CreateClient();

        var ct = TestContext.Current.CancellationToken;
        var url = new Uri($"{Constants.BaseUrl}/buyandsell?fromDate={Constants.StartMockDate}&toDate={Constants.EndMockDate}", UriKind.Relative);

        HttpResponseMessage? result = null;
        for (var i = 0; i < 10; i++)
        {
            result = await client.GetAsync(url, cancellationToken: ct);
        }

        result?.StatusCode.ShouldBe(expectedStatusCode);
    }
}
