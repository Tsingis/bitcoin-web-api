using E2ETests.Setup;
using Microsoft.Playwright;
using Xunit;

namespace E2ETests;

public class SwaggerTests : E2ETestBase
{
    [Fact]
    public async Task HasTitle()
    {
        var page = await Context.NewPageAsync();

        await page.GotoAsync("/swagger");

        await Assertions.Expect(page).ToHaveTitleAsync("Swagger UI");
    }

    [Fact]
    public async Task HasEndpoints()
    {
        var page = await Context.NewPageAsync();

        await page.GotoAsync("/swagger");

        var endpointsList = page.Locator(".operation-tag-content > *");

        await Assertions.Expect(endpointsList).ToHaveCountAsync(3);
    }
}
