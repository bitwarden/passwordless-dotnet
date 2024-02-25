using System.Collections.Generic;

namespace Passwordless.Models;

/// <summary>
/// Response from GetEventLog. Contains list of events for the application.
/// </summary>
public class GetEventLogResponse
{
    /// <summary>
    /// Name of application the events correspond to.
    /// </summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// List of events for the application based on the request pagination parameters.
    /// This will always be sorted by PerformedAt in descending order.
    /// </summary>
    public IReadOnlyList<ApplicationEvent> Events { get; set; } = new List<ApplicationEvent>();

    /// <summary>
    /// Total number of events for the application.
    /// </summary>
    public int TotalEventCount { get; set; }
}