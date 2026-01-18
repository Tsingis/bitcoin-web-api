using Microsoft.Playwright;
using Xunit;

namespace E2ETests.Setup;

public abstract class E2ETestBase : IAsyncLifetime
{
    private readonly TestApplicationHost _host = new();
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _context;

    protected IPlaywright Playwright =>
        _playwright ?? throw new InvalidOperationException("Playwright not initialized");

    protected IBrowser Browser =>
        _browser ?? throw new InvalidOperationException("Browser not initialized");

    protected IBrowserContext Context =>
        _context ?? throw new InvalidOperationException("Browser context not initialized");

    public async ValueTask InitializeAsync()
    {
        await _host.InitializeAsync();

        _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new() { Headless = true });

        _context = await _browser.NewContextAsync(new()
        {
            BaseURL = _host.BaseAddress?.ToString()
        });
    }

    public async ValueTask DisposeAsync()
    {
        if (_context is not null)
        {
            await _context.CloseAsync();
        }

        if (_browser is not null)
        {
            await _browser.CloseAsync();
        }

        _playwright?.Dispose();
        await _host.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
