using Api.Setup;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Services;
using Shouldly;
using Xunit;

namespace ArchitectureTests;

public class ServiceRegistrationTests
{
    [Fact]
    public void Scoped_MarketClient()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddServices(builder.Environment);

        var marketClient = builder.Services.SingleOrDefault(x => x.ServiceType == typeof(IMarketClient));

        marketClient.ShouldNotBeNull("Service is not registered");
        marketClient.Lifetime.ShouldBe(ServiceLifetime.Scoped);
    }

    [Fact]
    public void Scoped_MarketService()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddServices(builder.Environment);

        var marketService = builder.Services.SingleOrDefault(x => x.ServiceType == typeof(IMarketService));

        marketService.ShouldNotBeNull("Service is not registered");
        marketService.Lifetime.ShouldBe(ServiceLifetime.Scoped);
    }
}
