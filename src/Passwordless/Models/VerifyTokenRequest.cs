namespace Passwordless.Models;

internal class VerifyTokenRequest
{
    public VerifyTokenRequest(string token)
    {
        Token = token;
    }

    public string Token { get; }
}