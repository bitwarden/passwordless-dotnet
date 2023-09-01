using System.Text.Json.Serialization;

namespace Passwordless.Net.Models;

public class RegisterTokenResponse
{
    public RegisterTokenResponse(string token)
    {
        Token = token;
    }

    public string Token { get; }
}