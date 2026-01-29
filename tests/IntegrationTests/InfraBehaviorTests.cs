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
    public async Task HitsCache(bool useOutputCache, bool useRateLimiter, bool expectedCacheHit)
    {
        var ct = TestContext.Current.CancellationToken;
        using var factory = new TestFactory(fixture, useOutputCache, useRateLimiter);
        var client = factory.CreateClient();

        var url = Utility.BuildUri("buyandsell", Constants.StartMockDate, Constants.EndMockDate);

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
    public async Task HitsRateLimit(bool useOutputCache, bool useRateLimiter, HttpStatusCode expectedStatusCode)
    {
        var ct = TestContext.Current.CancellationToken;
        using var factory = new TestFactory(fixture, useOutputCache, useRateLimiter);
        var client = factory.CreateClient();

        var url = Utility.BuildUri("buyandsell", Constants.StartMockDate, Constants.EndMockDate);

        HttpResponseMessage? result = null;
        for (var i = 0; i < 10; i++)
        {
            result = await client.GetAsync(url, cancellationToken: ct);
        }

        result?.StatusCode.ShouldBe(expectedStatusCode);
    }
}
