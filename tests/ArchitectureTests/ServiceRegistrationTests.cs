using Api.Setup;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Services;
using Xunit;

namespace ArchitectureTests;

public class ServiceRegistrationTests
{
    [Fact]
    public void ShouldHave_Scoped_MarketClient()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.ConfigureServices();

        var marketClient = builder.Services.SingleOrDefault(x => x.ServiceType == typeof(IMarketClient));

        Assert.NotNull(marketClient);
        Assert.Equal(ServiceLifetime.Scoped, marketClient.Lifetime);
    }

    [Fact]
    public void ShouldHave_Scoped_MarketService()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.ConfigureServices();

        var marketService = builder.Services.SingleOrDefault(x => x.ServiceType == typeof(IMarketService));

        Assert.NotNull(marketService);
        Assert.Equal(ServiceLifetime.Scoped, marketService.Lifetime);
    }
}
