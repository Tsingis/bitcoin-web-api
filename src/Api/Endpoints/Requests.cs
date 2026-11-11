using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

public record DateRangeRequest(
    [FromQuery(Name = "fromDate")] DateOnly FromDate,
    [FromQuery(Name = "toDate")] DateOnly ToDate
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (FromDate >= ToDate)
        {
            yield return new ValidationResult(
                "fromDate must be before toDate.",
                [nameof(FromDate), nameof(ToDate)]);
        }
    }
}
