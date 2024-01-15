using System;

namespace Passwordless;

/// <summary>
/// The passkey credential stored by Passwordless.
/// <see href="https://www.w3.org/TR/webauthn-2/#public-key-credential"/>
/// </summary>
public class Credential(CredentialDescriptor descriptor, byte[] publicKey, byte[] userHandle, uint signatureCounter,
    string attestationFmt, DateTime createdAt, Guid aaGuid, DateTime lastUsedAt, string rpId,
    string origin, string country, string device, string nickname, string userId)
{
    /// <summary>
    /// Descriptor of the credential as defined by the WebAuthn specification
    /// <see href="https://w3c.github.io/webauthn/#enumdef-publickeycredentialtype" />
    /// </summary>
    public CredentialDescriptor Descriptor { get; } = descriptor;

    /// <summary>
    /// Public key of the passkey pair.
    /// </summary>
    public byte[] PublicKey { get; } = publicKey;

    /// <summary>
    /// Byte array of user identifier
    /// </summary>
    public byte[] UserHandle { get; } = userHandle;

    /// <summary>
    /// WebAuthn SignatureCounter, used for anti forgery.
    /// </summary>
    public uint SignatureCounter { get; } = signatureCounter;

    /// <summary>
    /// Attestation Statement format used to create credential
    /// </summary>
    public string AttestationFmt { get; } = attestationFmt;

    /// <summary>
    /// When the credential was created
    /// </summary>
    public DateTime CreatedAt { get; } = createdAt;

    /// <summary>
    /// The AAGUID of the authenticator. Can be used to identify the make and model of the authenticator.
    /// <see href="https://www.w3.org/TR/webauthn/#aaguid"/>
    /// </summary>
    public Guid AaGuid { get; } = aaGuid;

    /// <summary>
    /// Last time credential was used
    /// </summary>
    public DateTime LastUsedAt { get; } = lastUsedAt;

    /// <summary>
    /// Relying Party identifier
    /// <see href="https://w3c.github.io/webauthn/#dom-publickeycredentialcreationoptions-rp"/>
    /// </summary>
    public string RpId { get; } = rpId;

    /// <summary>
    /// Domain credential was created for
    /// </summary>
    public string Origin { get; } = origin;

    /// <summary>
    /// Optional country credential was created in
    /// </summary>
    public string Country { get; } = country;

    /// <summary>
    /// Device the credential was created on
    /// </summary>
    public string Device { get; } = device;

    /// <summary>
    /// Friendly name for credential.
    /// </summary>
    public string Nickname { get; } = nickname;

    /// <summary>
    /// Identifier for the user
    /// </summary>
    public string UserId { get; } = userId;

    /// <summary>
    /// Whether the credential is synced (or backed up or not).
    /// </summary>
    public bool? BackupState { get; set; }

    /// <summary>
    /// Whether the credential is eligible for backup or syncing
    /// </summary>
    public bool? IsBackupEligible { get; set; }

    /// <summary>
    /// Whether the credential is discoverable
    /// </summary>
    public bool? IsDiscoverable { get; set; }
}