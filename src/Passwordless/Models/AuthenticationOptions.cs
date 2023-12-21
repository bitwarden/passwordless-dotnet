namespace Passwordless.Models;

public class AuthenticationOptions
{
    public string UserId { get; }

    public AuthenticationOptions(string userId)
    {
        UserId = userId;
    }
}