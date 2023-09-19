namespace Passwordless.Net.Models;

internal class DeleteUserRequest
{
    public DeleteUserRequest(string userId)
    {
        UserId = userId;
    }

    public string UserId { get; }
}