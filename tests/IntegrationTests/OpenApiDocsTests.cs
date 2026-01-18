using System.Net;
using System.Text.Json;
using IntegrationTests.Setup;
using Shouldly;
using Xunit;

namespace IntegrationTests;

public sealed class OpenApiDocsTests(Fixture fixture, ITestOutputHelper outputHelper) : IntegrationTestBase(fixture, outputHelper)
{
    [Theory]
    [InlineData("/openapi/v1.json")]
    public async Task DocsJson(string endpoint)
    {
        var ct = TestContext.Current.CancellationToken;
        var result = await _client.GetAsync(new Uri(endpoint, UriKind.Relative), cancellationToken: ct);
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
}
