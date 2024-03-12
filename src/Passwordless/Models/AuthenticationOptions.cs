using System;

namespace Passwordless.Models;

/// <summary>
/// Used to manually generate an authentication token.
/// </summary>
/// <param name="UserId">User identifier the token is intended for.</param>
/// <param name="TimeToLive">Optional. How long a token is valid for. Default value is 15 minutes.</param>
public record AuthenticationOptions(string UserId, TimeSpan? TimeToLive = null);

internal record AuthenticationOptionsRequest(string UserId, int? TimeToLive = null);