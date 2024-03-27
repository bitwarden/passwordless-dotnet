namespace Passwordless.Models;

/// <summary>
/// Request for getting the event logs for an application.
/// </summary>
/// <param name="PageNumber">Page number for retrieving event log records.</param>
/// <param name="NumberOfResults">This is the max number of results that will be returned. Must be between 1-1000.</param>
public record GetEventLogRequest(int PageNumber, int? NumberOfResults = null);