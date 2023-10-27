namespace Passwordless;

/// <summary>
/// The passkey credential stored by Passwordless
/// <see href="https://www.w3.org/TR/webauthn-2/#public-key-credential"/>
/// </summary>
public class Credential
{
    /// <summary>
    /// Initializes an instance of <see cref="Credential"/>
    /// </summary>
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

    /// <summary>
    /// Descriptor of the credential as defined by the WebAuthn specification
    /// <see href="https://w3c.github.io/webauthn/#enumdef-publickeycredentialtype" />
    /// </summary>
    public CredentialDescriptor Descriptor { get; }

    /// <summary>
    /// Public key of the passkey pair.
    /// </summary>
    public byte[] PublicKey { get; }

    /// <summary>
    /// Byte array of user identifier
    /// </summary>
    public byte[] UserHandle { get; }

    /// <summary>
    /// Number of times credential has been used
    /// </summary>
    public uint SignatureCounter { get; }

    /// <summary>
    /// Attestation Statement format used to create credential
    /// </summary>
    public string AttestationFmt { get; }

    /// <summary>
    /// When the credential was created 
    /// </summary>
    public DateTime CreatedAt { get; }

    /// <summary>
    /// The AAGUID of the authenticator. Can be used to identify the make and model of the authenticator.
    /// <see href="https://www.w3.org/TR/webauthn/#aaguid"/>
    /// </summary>
    public Guid AaGuid { get; }

    /// <summary>
    /// Last time credential was used
    /// </summary>
    public DateTime LastUsedAt { get; }

    /// <summary>
    /// Relying Party identifier
    /// <see href="https://w3c.github.io/webauthn/#dom-publickeycredentialcreationoptions-rp"/>
    /// </summary>
    public string RPID { get; }

    /// <summary>
    /// Domain credential was created for
    /// </summary>
    public string Origin { get; }

    /// <summary>
    /// Optional country credential was created in
    /// </summary>
    public string Country { get; }

    /// <summary>
    /// Device the credential was created on
    /// </summary>
    public string Device { get; }

    /// <summary>
    /// Friendly name for credential.
    /// </summary>
    public string Nickname { get; }

    /// <summary>
    /// Identifier for the user
    /// </summary>
    public string UserId { get; }
}