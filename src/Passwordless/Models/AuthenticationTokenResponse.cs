namespace Passwordless.Models;

public class AuthenticationTokenResponse
{
    public AuthenticationTokenResponse(string token)
    {
        Token = token;
    }

    public string Token { get; }
}