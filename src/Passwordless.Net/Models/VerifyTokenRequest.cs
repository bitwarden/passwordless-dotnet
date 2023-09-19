namespace Passwordless.Net.Models;

internal class VerifyTokenRequest
{
    public VerifyTokenRequest(string token)
    {
        Token = token;
    }

    public string Token { get; }
}