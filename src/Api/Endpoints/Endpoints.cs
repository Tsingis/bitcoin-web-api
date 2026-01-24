using Api.Endpoints.Bitcoin;
using Asp.Versioning;

namespace Api.Endpoints;

internal static class Endpoints
{
    internal static void MapEndpoints(this WebApplication app)
    {
        var apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        var api = app.MapGroup("api/v{version:apiVersion}")
            .WithApiVersionSet(apiVersionSet);

        api.MapBitcoinGroup();
    }
}
