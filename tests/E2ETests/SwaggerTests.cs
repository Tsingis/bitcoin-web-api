using E2ETests.Setup;
using Microsoft.Playwright;
using Xunit;

namespace E2ETests;

public sealed class SwaggerTests : E2ETestBase
{
    [Fact]
    public async Task HasTitle()
    {
        var page = await Context.NewPageAsync();

        await page.GotoAsync("/swagger");

        await Assertions.Expect(page).ToHaveTitleAsync("Swagger UI");
    }
}
