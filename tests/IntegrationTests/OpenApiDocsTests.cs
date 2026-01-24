using System.Net;
using System.Text.Json;
using IntegrationTests.Setup;
using Shouldly;
using Xunit;

namespace IntegrationTests;

public sealed class OpenApiDocsTests(WiremockFixture fixture, ITestOutputHelper outputHelper) : TestBase(fixture, outputHelper)
{
    public const string DocsUrl = "/openapi/v1.json";

    [Fact]
    public async Task EndpointHasExpectedCustomExamples()
    {
        var ct = TestContext.Current.CancellationToken;
        var result = await _client.GetAsync(new Uri(DocsUrl, UriKind.Relative), ct);
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        var data = await result.Content.ReadAsStringAsync(ct);

        Should.NotThrow(() =>
        {
            using var document = JsonDocument.Parse(data);
            var root = document.RootElement;

            root.TryGetProperty("openapi", out var _).ShouldBeTrue("Missing 'openapi'");

            var firstPath = root.GetProperty("paths").EnumerateObject().First(x => x.Name.Contains("bitcoin"));
            var parameters = firstPath.Value.GetProperty("get").GetProperty("parameters");

            foreach (var param in parameters.EnumerateArray())
            {
                if (param.TryGetProperty("example", out var example))
                {
                    param.TryGetProperty("name", out var name);

                    if (name.GetString() == "fromDate")
                    {
                        example.ToString().ShouldBe("2025-08-30");
                    }

                    if (name.GetString() == "toDate")
                    {
                        example.ToString().ShouldBe("2025-09-10");
                    }
                }
            }
        });
    }

    [Fact]
    public async Task AllEndpointsHaveExpectedResponses()
    {
        var ct = TestContext.Current.CancellationToken;
        var result = await _client.GetAsync(new Uri(DocsUrl, UriKind.Relative), ct);
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        var data = await result.Content.ReadAsStringAsync(ct);

        using var document = JsonDocument.Parse(data);
        var root = document.RootElement;

        root.TryGetProperty("paths", out var paths).ShouldBeTrue("Missing 'paths'");

        var expectedStatusCodes = new[]
        {
            HttpStatusCode.OK,
            HttpStatusCode.NoContent,
            HttpStatusCode.BadRequest,
            HttpStatusCode.Unauthorized,
            HttpStatusCode.TooManyRequests,
            HttpStatusCode.InternalServerError
        }.Cast<int>().ToArray();

        foreach (var path in paths.EnumerateObject())
        {
            foreach (var operation in path.Value.EnumerateObject())
            {
                var method = operation.Name;

                operation.Value.TryGetProperty("responses", out var responses)
                    .ShouldBeTrue($"Missing 'responses' for {method.ToUpper()} {path.Name}");

                var actualResponseCodes = responses
                    .EnumerateObject()
                    .Select(x => int.Parse(x.Name))
                    .OrderBy(x => x)
                    .ToArray();

                actualResponseCodes.ShouldBe(expectedStatusCodes, $"Unexpected responses for {method.ToUpper()} {path.Name}");
            }
        }
    }

}
