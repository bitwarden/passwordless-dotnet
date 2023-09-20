namespace Passwordless.Net.Models;

public class AddAliasRequest
{
    public AddAliasRequest(string userId, string alias, bool hashing = true)
        : this(userId, hashing)
    {
        if (string.IsNullOrWhiteSpace(alias)) throw new ArgumentException($"'{nameof(alias)}' cannot be null, empty or whitespace.");
        Aliases = new HashSet<string>
        {
            alias ?? throw new ArgumentNullException(nameof(alias))
        };
    }

    public AddAliasRequest(string userId, HashSet<string> aliases, bool hashing = true)
        : this(userId, hashing)
    {
        if (aliases == null || !aliases.Any()) throw new ArgumentException($"'{nameof(aliases)}' cannot be null or empty.");
        if (aliases.Any(string.IsNullOrWhiteSpace)) throw new ArgumentException("One of the aliases is null, empty or whitespace");
        Aliases = aliases;
    }

    private AddAliasRequest(string userId, bool hashing = true)
    {
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        Hashing = hashing;
        Aliases = new HashSet<string>();
    }

    public string UserId { get; }
    public HashSet<string> Aliases { get; }

    /// <summary>
    /// If you want your aliases to be available in plain text, set the <see cref="bool"/> false.
    /// </summary>
    public bool Hashing { get; } = true;
}