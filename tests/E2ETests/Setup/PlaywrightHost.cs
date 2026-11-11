using Microsoft.Playwright;

namespace E2ETests.Setup;

internal static class PlaywrightHost
{
    private static readonly Lazy<Task<IPlaywright>> s_playwright =
        new(Microsoft.Playwright.Playwright.CreateAsync);

    private static readonly Lazy<Task<IBrowser>> s_browser =
        new(async () =>
        {
            var pw = await s_playwright.Value;
            return await pw.Chromium.LaunchAsync(new() { Headless = true });
        });

    public static Task<IPlaywright> Playwright => s_playwright.Value;
    public static Task<IBrowser> Browser => s_browser.Value;
}
