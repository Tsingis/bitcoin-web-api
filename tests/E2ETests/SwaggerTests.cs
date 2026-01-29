using E2ETests.Setup;
using Microsoft.Playwright;
using Xunit;

namespace E2ETests;

public class SwaggerTests(TestFactory factory) : TestBase(factory)
{
    [Fact]
    public async Task HasTitle()
    {
        var page = await Context.NewPageAsync();

        await page.GotoAsync("/swagger");

        await Assertions.Expect(page).ToHaveTitleAsync("Bitcoin Web API");
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
