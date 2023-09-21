namespace Passwordless.Models;

public class AddAliasRequest
{
    public AddAliasRequest(string userId, bool hashing = true)
        : this(userId, new HashSet<string>(), hashing)
    {
    }

    public AddAliasRequest(string userId, string alias, bool hashing = true)
        : this(userId, new HashSet<string> { alias }, hashing)
    {
    }

    public AddAliasRequest(string userId, HashSet<string> aliases, bool hashing = true)
    {
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        Hashing = hashing;

        Aliases = aliases == null
            ? new HashSet<string>()
            : new HashSet<string>(aliases.Where(x => !string.IsNullOrWhiteSpace(x)));
    }

    public string UserId { get; }

    public HashSet<string> Aliases { get; }

    /// <summary>
    /// If you want your aliases to be available in plain text, set the <see cref="bool"/> false.
    /// </summary>
    public bool Hashing { get; }
}