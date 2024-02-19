namespace Passwordless.Models;

/// <summary>
/// Used to manually generate an authentication token.
/// </summary>
/// <param name="UserId">User identifier the token is intended for.</param>
/// <param name="TimeToLive">Number of seconds the token will be valid for.</param>
public record AuthenticationOptions(string UserId, int TimeToLive);