using E2ETests.Setup;
using Microsoft.Playwright;
using Xunit;

namespace E2ETests;

public sealed class ScalarTests : E2ETestBase
{
    [Fact]
    public async Task HasTitle()
    {
        var page = await Context.NewPageAsync();

        await page.GotoAsync("/scalar");

        await Assertions.Expect(page).ToHaveTitleAsync("Bitcoin Web API v1");
    }
}
