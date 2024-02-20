using System;

namespace Passwordless.Models;

/// <summary>
/// Request for sending an email with a link that contains a 1-time use token to be used for validating signin.
/// </summary>
/// <param name="EmailAddress">Valid email address that will be the recipient of the magic link email</param>
/// <param name="UrlTemplate">Url template that needs to contain the token template, <token>. The token template will be replaced with a valid signin token to be sent to the verify sign in token endpoint (https://v4.passwsordless.dev/signin/verify).</param>
/// <param name="UserId">Identifier for the user the email is intended for.</param>
/// <param name="TimeToLive">Length of time the magic link will be active. Default value will be 15 minutes.</param>
public record SendMagicLinkRequest(string EmailAddress, string UrlTemplate, string UserId, TimeSpan? TimeToLive);