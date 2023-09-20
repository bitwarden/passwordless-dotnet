namespace Passwordless;

public class Credential
{
    public Credential(CredentialDescriptor descriptor, byte[] publicKey, byte[] userHandle, uint signatureCounter,
        string attestationFmt, DateTime createdAt, Guid aaGuid, DateTime lastUsedAt, string rpid,
        string origin, string country, string device, string nickname, string userId)
    {
        Descriptor = descriptor;
        PublicKey = publicKey;
        UserHandle = userHandle;
        SignatureCounter = signatureCounter;
        AttestationFmt = attestationFmt;
        CreatedAt = createdAt;
        AaGuid = aaGuid;
        LastUsedAt = lastUsedAt;
        RPID = rpid;
        Origin = origin;
        Country = country;
        Device = device;
        Nickname = nickname;
        UserId = userId;
    }

    public CredentialDescriptor Descriptor { get; }
    public byte[] PublicKey { get; }
    public byte[] UserHandle { get; }
    public uint SignatureCounter { get; }
    public string AttestationFmt { get; }
    public DateTime CreatedAt { get; }
    public Guid AaGuid { get; }
    public DateTime LastUsedAt { get; }
    public string RPID { get; }
    public string Origin { get; }
    public string Country { get; }
    public string Device { get; }
    public string Nickname { get; }
    public string UserId { get; }
}