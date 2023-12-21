using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Passwordless.Example;

[ApiController]
public class PasswordlessController : Controller
{
    private readonly IPasswordlessClient _passwordlessClient;

    public PasswordlessController(IPasswordlessClient passwordlessClient)
    {
        _passwordlessClient = passwordlessClient;
    }

    /// <summary>
    ///     Register - Get token from the passwordless API
    ///     The passwordless client side code needs a Token to register a key to a username.
    ///     The token is used by the Passwordless API to verify that this action is allowed for this user.
    ///     Your server can create this token by calling the Passwordless API with the ApiSecret.
    ///     This allows you to control the process, perhaps you only want to allow new users to register or only allow already
    ///     signed in users to add a Key to their own account.
    ///     Please see: https://docs.passwordless.dev/guide/api.html#register-token
    /// </summary>
    /// <param name="alias"></param>
    /// <returns></returns>
    [HttpGet("/create-token")]
    public async Task<IActionResult> GetRegisterToken(string alias)
    {
        var userId = Guid.NewGuid().ToString();

        var payload = new RegisterOptions(userId, alias)
        {
            Aliases = new HashSet<string> { alias }
        };

        try
        {
            var token = await _passwordlessClient.CreateRegisterTokenAsync(payload);
            return Ok(token);
        }
        catch (PasswordlessApiException e)
        {
            return new JsonResult(e.Details)
            {
                StatusCode = e.Details.Status
            };
        }
    }

    /// <summary>
    ///     Sign in - Verify the sign in
    ///     The passwordless API handles all the cryptography and WebAuthn details so that you don't need to.
    ///     In order for you to verify that the sign in was successful and retrieve details such as the username, you need to
    ///     verify the token that the passwordless client side code returned to you.
    ///     This is as easy as POST'ing it to together with your ApiSecret.
    ///     Please see: https://docs.passwordless.dev/guide/api.html#signin-verify
    /// </summary>
    /// <param name="token"></param>
    [HttpGet("/verify-signin")]
    public async Task<IActionResult> VerifyAuthenticationToken(string token)
    {
        try
        {
            var verifiedUser = await _passwordlessClient.VerifyAuthenticationTokenAsync(token);
            return Ok(verifiedUser);
        }
        catch (PasswordlessApiException e)
        {
            return new JsonResult(e.Details)
            {
                StatusCode = e.Details.Status
            };
        }
    }
}