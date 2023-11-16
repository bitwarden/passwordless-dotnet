using System;
using System.Collections.Generic;

namespace Passwordless;

public class PasswordlessUserSummary
{
    public PasswordlessUserSummary(string userId, IReadOnlyList<string> aliases, int credentialsCount,
        int aliasCount, DateTime? lastUsedAt)
    {
        UserId = userId;
        Aliases = aliases;
        CredentialsCount = credentialsCount;
        AliasCount = aliasCount;
        LastUsedAt = lastUsedAt;
    }

    public string UserId { get; }
    public IReadOnlyList<string> Aliases { get; }
    public int CredentialsCount { get; }
    public int AliasCount { get; }
    public DateTime? LastUsedAt { get; }
}