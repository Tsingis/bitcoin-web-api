using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

public record DateRangeRequest(
    [FromQuery(Name = "fromDate")] DateOnly FromDate,
    [FromQuery(Name = "toDate")] DateOnly ToDate
);
