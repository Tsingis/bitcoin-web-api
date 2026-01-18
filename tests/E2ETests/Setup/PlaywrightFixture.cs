using Microsoft.Playwright;

namespace E2ETests.Setup;

internal static class PlaywrightFixture
{
    private static readonly Lazy<Task<IPlaywright>> _playwright =
        new(Microsoft.Playwright.Playwright.CreateAsync);

    private static readonly Lazy<Task<IBrowser>> _browser =
        new(async () =>
        {
            var pw = await _playwright.Value;
            return await pw.Chromium.LaunchAsync(new() { Headless = true });
        });

    public static Task<IPlaywright> Playwright => _playwright.Value;
    public static Task<IBrowser> Browser => _browser.Value;
}
