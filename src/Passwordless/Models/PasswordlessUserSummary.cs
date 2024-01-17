using System;
using System.Collections.Generic;

namespace Passwordless;

public record PasswordlessUserSummary(
    string UserId,
    IReadOnlyList<string> Aliases,
    int CredentialsCount,
    int AliasCount,
    DateTime? LastUsedAt
);