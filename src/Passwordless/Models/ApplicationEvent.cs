using System;

namespace Passwordless.Models;

/// <summary>
/// An event that occured using Passwordless library.
/// </summary>
/// <param name="Id">Event ID.</param>
/// <param name="PerformedAt">When the event was performed.</param>
/// <param name="EventType">The type of event. <see href="https://github.com/passwordless/passwordless-server/blob/main/src/Common/EventLog/Enums/EventType.cs"/></param>
/// <param name="Message">Description of the event.</param>
/// <param name="Severity">Severity of the event. <see href="https://github.com/passwordless/passwordless-server/blob/main/src/Common/EventLog/Enums/Severity.cs"/></param>
/// <param name="Subject">The target of the event. Can be in reference to a user or the application.</param>
/// <param name="ApiKeyId">Last 4 characters of the api key (public/secret) used to perform the event.</param>
public record ApplicationEvent(
    Guid Id,
    DateTime PerformedAt,
    string EventType,
    string Message,
    string Severity,
    string Subject,
    string ApiKeyId
);