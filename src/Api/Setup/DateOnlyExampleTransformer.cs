using System.Text.Json.Nodes;
using Common;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Api.Setup;

public sealed class DateOnlyExampleTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(operation);
        ArgumentNullException.ThrowIfNull(context);

        var usesDateExample = context.Description.ActionDescriptor.EndpointMetadata
            .OfType<UseDateExamplesAttribute>()
            .Any();

        if (!usesDateExample)
        {
            return Task.CompletedTask;
        }

        const string DateFormat = "yyyy-MM-dd";

        var fromDate = EnvVarAccessors.UseMockServer
                ? new DateOnly(2025, 8, 30)
                : DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));

        ReplaceDateExample(operation, "fromDate", fromDate.ToString(DateFormat));

        var toDate = EnvVarAccessors.UseMockServer
                        ? new DateOnly(2025, 9, 10)
                        : DateOnly.FromDateTime(DateTime.UtcNow);

        ReplaceDateExample(operation, "toDate", toDate.ToString(DateFormat));

        return Task.CompletedTask;
    }

    private static void ReplaceDateExample(OpenApiOperation operation, string parameterName, string value)
    {
        if (operation.Parameters is null)
        {
            return;
        }

        for (var i = 0; i < operation.Parameters.Count; i++)
        {
            var old = operation.Parameters[i];

            if (old.Name != parameterName)
            {
                continue;
            }

            operation.Parameters[i] = new OpenApiParameter
            {
                Name = old.Name,
                In = old.In,
                Description = old.Description,
                Required = old.Required,
                Style = old.Style,
                Explode = old.Explode,
                AllowReserved = old.AllowReserved,
                Schema = old.Schema,
                Examples = old.Examples,
                Example = JsonValue.Create(value),
                Content = old.Content,
                Extensions = old.Extensions,
                AllowEmptyValue = old.AllowEmptyValue,
                Deprecated = old.Deprecated,
            };
        }
    }
}
