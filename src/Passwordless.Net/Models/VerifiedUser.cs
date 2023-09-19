namespace Passwordless.Net;

public class VerifiedUser
{
    public VerifiedUser(string userId, byte[] credentialId, bool success,
        DateTime timestamp, string rpId, string origin, string device,
        string country, string nickname, DateTime expiresAt, Guid tokenId,
        string type)
    {
        UserId = userId;
        CredentialId = credentialId;
        Success = success;
        Timestamp = timestamp;
        RpId = rpId;
        Origin = origin;
        Device = device;
        Country = country;
        Nickname = nickname;
        ExpiresAt = expiresAt;
        TokenId = tokenId;
        Type = type;
    }
    public string UserId { get; }
    public byte[] CredentialId { get; }
    public bool Success { get; }
    public DateTime Timestamp { get; }
    public string RpId { get; }
    public string Origin { get; }
    public string Device { get; }
    public string Country { get; }
    public string Nickname { get; }
    public DateTime ExpiresAt { get; }
    public Guid TokenId { get; }
    public string Type { get; }
}