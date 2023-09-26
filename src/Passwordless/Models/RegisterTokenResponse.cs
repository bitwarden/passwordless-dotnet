namespace Passwordless.Models;

public class RegisterTokenResponse
{
    public RegisterTokenResponse(string token)
    {
        Token = token;
    }

    public string Token { get; }
}