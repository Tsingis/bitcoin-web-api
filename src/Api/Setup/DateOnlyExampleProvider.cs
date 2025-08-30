
using System.Text.Json.Serialization;
using MartinCostello.OpenApi;

namespace Api.Setup;

public sealed class DateOnlyExampleProvider : IExampleProvider<DateOnly>
{
    public static DateOnly GenerateExample() => DateOnly.FromDateTime(DateTime.UtcNow);
}

[JsonSerializable(typeof(DateOnly))]
[JsonSourceGenerationOptions(
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    NumberHandling = JsonNumberHandling.Strict,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    WriteIndented = true)]
public sealed partial class DateOnlyJsonSerializerContext : JsonSerializerContext;
