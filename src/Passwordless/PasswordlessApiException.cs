using System;

namespace Passwordless;

/// <summary>
/// Exception thrown when the API returns a non-successful response.
/// </summary>
public sealed class PasswordlessApiException(PasswordlessProblemDetails details) : Exception(details.Title)
{
    /// <summary>
    /// Details associated with the problem, returned by the API.
    /// </summary>
    public PasswordlessProblemDetails Details { get; } = details;
}