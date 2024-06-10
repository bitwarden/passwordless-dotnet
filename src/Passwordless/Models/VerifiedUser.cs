using System;

namespace Passwordless;

public record VerifiedUser(
    string UserId,
    byte[] CredentialId,
    bool Success,
    DateTime Timestamp,
    string RpId,
    string Origin,
    string Device,
    string Country,
    string Nickname,
    DateTime ExpiresAt,
    Guid TokenId,
    string Type,
    string Purpose
);