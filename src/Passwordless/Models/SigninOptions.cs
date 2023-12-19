namespace Passwordless.Models;

public class SigninOptions
{
    public string UserId { get; }

    public SigninOptions(string userId)
    {
        UserId = userId;
    }
}