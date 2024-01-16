using System.Collections.Generic;

namespace Passwordless.Models;

/// <summary>
/// Response from GetEventLog. Contains list of events for the application.
/// </summary>
/// <param name="TenantId">Name of application the events correspond to.</param>
/// <param name="Events">List of events for the application based on the request pagination parameters. This will always be sorted by PerformedAt in descending order.</param>
/// <param name="TotalEventCount">Total number of events for the application.</param>
public record GetEventLogResponse(
    string TenantId,
    IReadOnlyList<ApplicationEvent> Events,
    int TotalEventCount
);