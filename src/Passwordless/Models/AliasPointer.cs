namespace Passwordless;

/// <summary>
/// Alias used for asserting a user.
/// </summary>
public record AliasPointer(string UserId, string Alias, string Plaintext);