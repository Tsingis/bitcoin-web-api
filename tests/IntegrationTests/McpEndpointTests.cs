using System.Text.Json;
using IntegrationTests.Setup;
using ModelContextProtocol.Client;
using Shouldly;
using Xunit;

namespace IntegrationTests;

public sealed class McpEndpointTests(WiremockFixture fixture, ITestOutputHelper outputHelper)
    : TestBase(fixture, outputHelper)
{
    [Fact]
    public async Task Tools_DiscoverBitcoinMarketAnalysis()
    {
        var ct = TestContext.Current.CancellationToken;
        await using var transport = CreateMcpTransport();
        await using var mcpClient = await McpClient.CreateAsync(transport, cancellationToken: ct);

        var tools = await mcpClient.ListToolsAsync(cancellationToken: ct);
        tools.Count.ShouldBe(1);
        tools[0].Description.ShouldContain("Analyzes Bitcoin market activity");
    }

    [Fact]
    public async Task AnalyzeBitcoinMarket_ReturnsStructuredResult()
    {
        var ct = TestContext.Current.CancellationToken;
        await using var transport = CreateMcpTransport();
        await using var mcpClient = await McpClient.CreateAsync(transport, cancellationToken: ct);
        var tools = await mcpClient.ListToolsAsync(cancellationToken: ct);

        var result = await mcpClient.CallToolAsync(
            tools[0].Name,
            new Dictionary<string, object?>
            {
                ["fromDate"] = Common.Constants.StartMockDate,
                ["toDate"] = Common.Constants.EndMockDate
            },
            cancellationToken: ct);

        result.IsError.ShouldNotBe(true, JsonSerializer.Serialize(result.Content));
        result.StructuredContent.ShouldNotBeNull();
        var output = JsonSerializer.SerializeToElement(result.StructuredContent);
        output.GetProperty("longestDownwardTrendDays").GetInt32().ShouldBe(3);
    }

    private HttpClientTransport CreateMcpTransport()
    {
        return new HttpClientTransport(
            new HttpClientTransportOptions
            {
                Endpoint = new Uri(_client.BaseAddress!, "/mcp"),
                TransportMode = HttpTransportMode.StreamableHttp
            },
            _client,
            null,
            false);
    }
}
