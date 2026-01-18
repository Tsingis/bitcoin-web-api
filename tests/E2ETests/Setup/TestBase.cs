using Microsoft.Playwright;
using Xunit;

namespace E2ETests.Setup;

public abstract class TestBase(Fixture fixture) : IAsyncLifetime
{
    protected IBrowserContext Context { get; private set; } = default!;

    public async ValueTask InitializeAsync()
    {
        var browser = await PlaywrightFixture.Browser;

        Context = await browser.NewContextAsync(new()
        {
            BaseURL = fixture.BaseAddress!.ToString()
        });
    }

    public async ValueTask DisposeAsync()
    {
        await Context.CloseAsync();
        GC.SuppressFinalize(this);
    }
}
