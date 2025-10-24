
using System.Text.Json.Serialization;
using Common;
using MartinCostello.OpenApi;

namespace Api.Setup;

public sealed class DateOnlyExampleProvider : IExampleProvider<DateOnly>
{
    public static DateOnly GenerateExample() => EnvVarAccessors.UseMockServer ? new DateOnly(2025, 8, 30) : DateOnly.FromDateTime(DateTime.UtcNow);
}

[JsonSerializable(typeof(DateOnly))]
[JsonSourceGenerationOptions(
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    NumberHandling = JsonNumberHandling.Strict,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    WriteIndented = true)]
public sealed partial class DateOnlyJsonSerializerContext : JsonSerializerContext;
