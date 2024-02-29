using System;

namespace Passwordless;

/// <summary>
/// The passkey credential stored by Passwordless.
/// <see href="https://www.w3.org/TR/webauthn-2/#public-key-credential"/>
/// </summary>
/// <param name="Descriptor">Descriptor of the credential as defined by the WebAuthn specification. <see href="https://w3c.github.io/webauthn/#enumdef-publickeycredentialtype" /></param>
/// <param name="PublicKey">Public key of the passkey pair.</param>
/// <param name="UserHandle">Byte array of user identifier.</param>
/// <param name="SignatureCounter">WebAuthn SignatureCounter, used for anti forgery.</param>
/// <param name="AttestationFmt">Attestation Statement format used to create credential.</param>
/// <param name="CreatedAt">When the credential was created.</param>
/// <param name="AaGuid">The AAGUID of the authenticator. Can be used to identify the make and model of the authenticator. <see href="https://www.w3.org/TR/webauthn/#aaguid"/></param>
/// <param name="LastUsedAt">Last time credential was used.</param>
/// <param name="RpId">Relying Party identifier. <see href="https://w3c.github.io/webauthn/#dom-publickeycredentialcreationoptions-rp"/></param>
/// <param name="Origin">Domain credential was created for.</param>
/// <param name="Country">Optional country credential was created in.</param>
/// <param name="Device">Device the credential was created on.</param>
/// <param name="Nickname">Friendly name for credential.</param>
/// <param name="UserId">Identifier for the user.</param>
/// <para name="BackupState">Whether the credential is synced (or backed up or not).</para>
/// <para name="IsBackupEligible">Whether the credential is eligible for backup or syncing.</para>
/// <para name="IsDiscoverable">Whether the credential is discoverable.</para>
public record Credential(
    CredentialDescriptor Descriptor,
    byte[] PublicKey,
    byte[] UserHandle,
    uint SignatureCounter,
    string AttestationFmt,
    DateTime CreatedAt,
    Guid AaGuid,
    DateTime LastUsedAt,
    string RpId,
    string Origin,
    string Country,
    string Device,
    string Nickname,
    string UserId,
    bool? BackupState = null,
    bool? IsBackupEligible = null,
    bool? IsDiscoverable = null);