using System;
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
    public string ApplicationName { get; init; } = string.Empty;
    /// <summary>
    /// List of events for the application based on the request pagination parameters. This will always be sorted by PerformedAt in descending order.
    /// </summary>
    public IEnumerable<ApplicationEvent> Events { get; init; } = new List<ApplicationEvent>();
    /// <summary>
    /// Total number of events for the application.
    /// </summary>
    public int TotalEventCount { get; init; }
}

/// <summary>
/// An event that occured using Passwordless library.
/// </summary>
public class ApplicationEvent
{
    public Guid Id { get; init; }
    /// <summary>
    /// When the record was performed. This will be in UTC.
    /// </summary>
    public DateTime PerformedAt { get; init; }

    /// <summary>
    /// The type of event <see href="https://github.com/passwordless/passwordless-server/blob/main/src/Common/EventLog/Enums/EventType.cs" />
    /// </summary>
    public string EventType { get; init; } = string.Empty;

    /// <summary>
    /// Description of the event
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Severity of the event <see href="https://github.com/passwordless/passwordless-server/blob/main/src/Common/EventLog/Enums/Severity.cs"/>
    /// </summary>
    public string Severity { get; init; } = string.Empty;

    /// <summary>
    /// The target of the event. Can be in reference to a user or the application.
    /// </summary>
    public string Subject { get; init; } = string.Empty;

    /// <summary>
    /// Last 4 characters of the api key (public/secret) used to perform the event.
    /// </summary>
    public string ApiKeyId { get; init; } = string.Empty;
}