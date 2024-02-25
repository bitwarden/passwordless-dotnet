using System.ComponentModel.DataAnnotations;

namespace Passwordless.Models;

/// <summary>
/// Request for getting the event logs for an application.
/// </summary>
public class GetEventLogRequest
{
    /// <summary>
    /// Page number for retrieving event log records.
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// This is the max number of results that will be returned. Must be between 1-1000.
    /// </summary>
    [Range(1, 1000)]
    public int? NumberOfResults { get; set; }
}