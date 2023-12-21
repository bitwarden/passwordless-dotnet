using System;

namespace Passwordless.Models;

/// <summary>
/// An event that occured using Passwordless library.
/// </summary>
public class ApplicationEvent
{
    public Guid Id { get; set; }
    /// <summary>
    /// When the record was performed. This will be in UTC.
    /// </summary>
    public DateTime PerformedAt { get; set; }

    /// <summary>
    /// The type of event <see href="https://github.com/passwordless/passwordless-server/blob/main/src/Common/EventLog/Enums/EventType.cs" />
    /// </summary>
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// Description of the event
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Severity of the event <see href="https://github.com/passwordless/passwordless-server/blob/main/src/Common/EventLog/Enums/Severity.cs"/>
    /// </summary>
    public string Severity { get; set; } = string.Empty;

    /// <summary>
    /// The target of the event. Can be in reference to a user or the application.
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Last 4 characters of the api key (public/secret) used to perform the event.
    /// </summary>
    public string ApiKeyId { get; set; } = string.Empty;
}