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

        var fromDate = EnvVarUtils.UseMockServer
            ? Constants.StartMockDate
            : Constants.Today.AddDays(-1);

        ReplaceDateExample(operation, "fromDate", fromDate.ToString(Constants.DateFormat));

        var toDate = EnvVarUtils.UseMockServer
            ? Constants.EndMockDate
            : Constants.Today;

        ReplaceDateExample(operation, "toDate", toDate.ToString(Constants.DateFormat));

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
