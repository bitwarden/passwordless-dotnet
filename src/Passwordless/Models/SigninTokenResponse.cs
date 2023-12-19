namespace Passwordless.Models;

public class SigninTokenResponse
{
    public SigninTokenResponse(string token)
    {
        Token = token;
    }

    public string Token { get; }
}