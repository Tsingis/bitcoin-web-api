using Microsoft.Playwright;
using Xunit;

namespace E2ETests.Setup;

public abstract class E2ETestBase : IAsyncLifetime
{
    protected ApiTestHost Host { get; } = new();
    protected IPlaywright Playwright { get; private set; } = null!;
    protected IBrowser Browser { get; private set; } = null!;
    protected IBrowserContext Context { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        await Host.InitializeAsync();

        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        Browser = await Playwright.Chromium.LaunchAsync(new() { Headless = true });

        Context = await Browser.NewContextAsync(new()
        {
            BaseURL = Host.BaseAddress.ToString()
        });
    }

    public async ValueTask DisposeAsync()
    {
        await Context.CloseAsync();
        await Browser.CloseAsync();
        Playwright.Dispose();
        await Host.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
