using E2ETests.Setup;
using Microsoft.Playwright;
using Xunit;

namespace E2ETests;

public class ScalarTests(Fixture fixture) : TestBase(fixture)
{
    [Fact]
    public async Task HasTitle()
    {
        var page = await Context.NewPageAsync();

        await page.GotoAsync("/scalar");

        await Assertions.Expect(page).ToHaveTitleAsync("Bitcoin Web API v1");
    }

    [Fact]
    public async Task HasEndpoints()
    {
        var page = await Context.NewPageAsync();

        await page.GotoAsync("/scalar");

        var endpointsList = page.Locator(".endpoints > *");

        await Assertions.Expect(endpointsList).ToHaveCountAsync(3);
    }
}
